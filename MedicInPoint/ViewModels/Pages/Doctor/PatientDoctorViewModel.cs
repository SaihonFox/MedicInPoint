using System.Collections.ObjectModel;

using Avalonia.Controls;
using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Medic.API.Refit.Placeholders;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.API.SignalR;
using MedicInPoint.Extensions;
using MedicInPoint.Models;
using MedicInPoint.Services;

using Microsoft.AspNetCore.SignalR.Client;

namespace MedicInPoint.ViewModels.Pages.Doctor;

public partial class PatientDoctorViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;
	private readonly INotificationService _notificationService = null!;
	private readonly IAppStateService _appService = null!;

	public PatientDoctorViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router, INotificationService notificationService, IAppStateService appService, MedicSignalRConnections connections) : this()
	{
		Title = "Пациенты";
		_router = router;
		_notificationService = notificationService;
		_appService = appService;

		FillAnalysisOrders();
		FillPatients();

		connections.PatientConnection.On<Patient>("PatientAdded", patient =>
		{
			if(patient.AnalysisOrders.FirstOrDefault(o => o.User == appService.CurrentUser) != null)
				AllPatients.Add(patient);
			OnSearchTextChanged(SearchText);
		});
		connections.PatientConnection.On<Patient>("PatientDeleted", patient =>
		{
			if (patient.AnalysisOrders.FirstOrDefault(o => o.User == appService.CurrentUser) != null)
				AllPatients.Remove(patient);
			OnSearchTextChanged(SearchText);
		});
	}

	public User CurrentUser => _appService.CurrentUser!;

	async void FillPatients()
	{
		using var response = await APIService.For<IPatient>().GetPatients();
		if (!response.IsSuccessStatusCode)
			return;

		foreach (var patient in response.Content!)
			AllPatients.Add(patient);
		Patients = [.. SearchPatientsText()];
	}

	async Task FillAnalysisOrders()
	{
		using var response = await APIService.For<IAnalysisOrder>().GetAnalysisOrders();
		if (!response.IsSuccessStatusCode)
			return;

		foreach(var order in response.Content!)
			AllOrders.Add(order);
	}

	[ObservableProperty]
	private string _searchText = string.Empty;

	partial void OnSearchTextChanged(string value)
	{
		var selectedPatient = SelectedPatient;

		if (value.IsNullOrWhiteSpace())
			Patients = [.. AllPatients];
		var patients = SearchPatientsText();
		if(!patients.SequenceEqual(Patients))
			Patients = [.. patients];

		if (Patients.FirstOrDefault(x => x.Id == selectedPatient?.Id) != null)
			SelectedPatient = selectedPatient;
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
	private ObservableCollection<Patient> _patients = [];

	[ObservableProperty]
	private ObservableCollection<AnalysisOrder> _allOrders = [];

	[ObservableProperty]
	private ObservableCollection<AnalysisOrder> _userOrders = [];

	[ObservableProperty]
	private Patient? _selectedPatient = null;

	[ObservableProperty]
	private int? _selectedPatientIndex = Design.IsDesignMode ? 0 : null;

	partial void OnSelectedPatientChanged(Patient? value)
	{
		if (value == null)
			return;

		UserOrders.Clear();
		foreach (var order in AllOrders.Where(x => x.PatientId == value.Id))
			UserOrders.Add(order);
	}
}