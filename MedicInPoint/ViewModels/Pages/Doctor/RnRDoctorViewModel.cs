using System.Collections.ObjectModel;

using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.SimpleRouter;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.API;
using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.API.SignalR;
using MedicInPoint.Extensions;
using MedicInPoint.Models;
using MedicInPoint.Services;

using Microsoft.AspNetCore.SignalR.Client;

namespace MedicInPoint.ViewModels.Pages.Doctor;

public partial class RnRDoctorViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;
	public readonly INotificationService _notificationService = null!;
	private readonly IAppStateService _appService = null!;

	public RnRDoctorViewModel(
		NestedHistoryRouter<ViewModelBase, MainViewModel> router,
		INotificationService notificationService,
		IAppStateService appService,
		MedicSignalRConnections connections
	) : this()
	{
		Title = "Запросы и запись";
		_router = router;
		_notificationService = notificationService;
		_appService = appService;

		FillPatients();
		FillRequests();
		FillAnalyses();

		connections.PatientConnection.On<Patient>("PatientAdded", patient =>
		{
			if (patient.AnalysisOrders.FirstOrDefault(o => o.User == appService.CurrentUser) != null)
				AllPatients.Add(patient);
			OnSearchTextChanged(SearchText);
		});

		PortCheckJob.PortStateChanged += (s, e) =>
		{
			Dispatcher.UIThread.Invoke(() => _notificationService.Show("connection", e.IsPortOpen.ToString()));
		};
	}

	#region Fill Methods
	async void FillPatients()
	{
		try
		{
			var response = await APIService.For<IPatient>().GetPatients();
			if (!response.IsSuccessful)
				return;

			AllPatients = [..response.Content];
			Patients = [.. AllPatients];
			//SelectedPatientIndex = 0;
			Log("none", "PatientDoctor");
		}
		catch (HttpRequestException ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "HttpError", this, $"Message: {ex.Message}\nStatus code: {ex.StatusCode}, RequestError: {ex.HttpRequestError}");
		}
		catch (Exception ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "Error", this, "Message: " + ex.Message + "\nsource: " + ex.GetType().FullName);
		}
	}

	async void FillRequests()
	{
		try
		{
			var response = await APIService.For<IRequest>().GetRequests();
			if (!response.IsSuccessful)
				return;

			AllRequests = [.. response.Content];
			Requests = [.. AllRequests];
			Log("none", "PatientDoctor");
		}
		catch (HttpRequestException ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "HttpError", this, $"Message: {ex.Message}\nStatus code: {ex.StatusCode}, RequestError: {ex.HttpRequestError}");
		}
		catch (Exception ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "Error", this, "Message: " + ex.Message + "\nsource: " + ex.GetType().FullName);
		}
	}
	#endregion Fill Methods

	[ObservableProperty]
	private string _searchText = string.Empty;

	partial void OnSearchTextChanged(string value)
	{
		var selectedPatient = SelectedPatient;
		if (value.IsNullOrWhiteSpace())
			Patients = [.. AllPatients];
		var patients = SearchPatientsText();
		if (!patients.SequenceEqual(Patients))
			Patients = [.. patients];

		if (Patients.Contains(selectedPatient))
			SelectedPatient = selectedPatient;
	}

	async void FillAnalyses()
	{
		try
		{
			var response = await APIService.For<IAnalysis>().GetAnalyses();
			if (!response.IsSuccessful)
				return;

			AllAnalyses = [.. response.Content];
			
			SelectedAnalysis = AllAnalyses[0];
			Log("none", "PatientDoctor");
		}
		catch (HttpRequestException ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "HttpError", this, $"Message: {ex.Message}\nStatus code: {ex.StatusCode}, RequestError: {ex.HttpRequestError}");
		}
		catch (Exception ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "Error", this, "Message: " + ex.Message + "\nsource: " + ex.GetType().FullName);
		}
	}

	partial void OnSelectedPatientChanged(Patient? value)
	{
		if (value == null)
			return;

		Requests = [.. AllRequests.Where(x => x.PatientId == value.Id)];
	}

	IEnumerable<Patient> SearchPatientsText() => AllPatients.Where(p => p.FullName.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase));

	[RelayCommand]
	private void Back() => _router.Back();

	[ObservableProperty]
	private ObservableCollection<Patient> _allPatients = !Design.IsDesignMode ? [] : [
			new Patient { Surname = "Тулькубаев", Name = "Ильнар", Sex = "Мужской", Phone = "89123456789", Birthday = new(2005, 04, 22), Passport = "8019123456" },
			new Patient { Surname = "Набиева", Name = "Лиана", Patronym = "Рамилевна", Sex = "Женский", Phone = "89098765432", Birthday = new(1999, 05, 25), Passport = "8020123456" },
		];

	[ObservableProperty]
	private ObservableCollection<Request> _allRequests = [];
	[ObservableProperty]
	private ObservableCollection<Patient> _patients = [];
	[ObservableProperty]
	private ObservableCollection<Request> _requests = [];

	[ObservableProperty]
	private Patient? _selectedPatient = null;

	[ObservableProperty]
	private int? _selectedPatientIndex = Design.IsDesignMode ? 0 : null;



	#region Record
	[ObservableProperty]
	private ObservableCollection<Analysis> _allAnalyses = [];

	[ObservableProperty]
	private Analysis? _selectedAnalysis = null;

	[ObservableProperty]
	private bool _isRecordButtonEnabled = true;

	[ObservableProperty]
	private AnalysisOrder _orderRecord = new();

	[ObservableProperty]
	private PatientAnalysisAddress _orderRecordAddress = new();

	[RelayCommand]
	private async Task NewOrder()
	{

	}

	#endregion Record
}