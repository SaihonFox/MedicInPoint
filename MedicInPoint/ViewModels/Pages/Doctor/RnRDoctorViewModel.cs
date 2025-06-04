using System.Collections.ObjectModel;
using System.Linq.Dynamic.Core;
using System.Net.NetworkInformation;
using System.Net.Sockets;

using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Logging;
using Avalonia.SimpleRouter;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Medic.API.Refit.Placeholders;

using MedicInPoint.API;
using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.API.SignalR;
using MedicInPoint.Extensions;
using MedicInPoint.Models;
using MedicInPoint.Services;

using Microsoft.AspNetCore.SignalR.Client;

using Newtonsoft.Json;

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

		AnalysesInRecord.CollectionChanged += (s, e) => AnalysesInRecordTotalPrice = AnalysesInRecord.Sum(x => x.Price);
	}

	public string CurrentUser => _appService.CurrentUser!.FullName;

	#region Fill Methods
	async Task FillPatients()
	{
		try
		{
			_notificationService.Show("Уведомление", "Загрузка списка пациентов");
			using var response = await APIService.For<IPatient>().GetPatients().ConfigureAwait(false);
			if (!response.IsSuccessful)
				return;

			AllPatients = [.. response.Content];
			Patients = [.. AllPatients];
		}
		catch (HttpRequestException ex) when (ex.GetBaseException() is SocketException sex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "SHttpError", this, $"|Patients|Message: {sex.Message}\nStatus code: {sex.ErrorCode}, RequestError: {sex.SocketErrorCode}");
			if (sex.SocketErrorCode == SocketError.ConnectionReset)
				Dispatcher.UIThread.Invoke(() => _notificationService.Show("Уведомление!", "Подключение прервано", NotificationType.Error));
		}
		catch (HttpRequestException ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "HttpError", this, $"|Patients|Message: {ex.Message}\nStatus code: {ex.StatusCode}, RequestError: {ex.HttpRequestError}");
			Dispatcher.UIThread.Invoke(() => _notificationService.Show("Уведомление!", ex.Message, NotificationType.Error));
		}
		catch (Exception ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "Error", this, $"{ex.GetBaseException().GetType().FullName}-{ex.HResult}|Message: {ex.Message}\nsource: {ex.GetType().FullName}");
		}
	}

	async Task FillRequests()
	{
		try
		{
			_notificationService.Show("Уведомление", "Загрузка списка запросов");
			using var response = await APIService.For<IRequest>().GetRequests().ConfigureAwait(false);
			if (!response?.IsSuccessful ?? false)
				return;

			AllRequests = [.. response.Content];
			Requests = [.. AllRequests.Where(x => x.DoctorId == _appService.CurrentUser!.Id)];
		}
		catch (HttpRequestException ex) when (ex.GetBaseException() is SocketException sex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "SHttpError", this, $"|Requests|Message: {sex.Message}\nStatus code: {sex.ErrorCode}, RequestError: {sex.SocketErrorCode}");
			if (sex.SocketErrorCode == SocketError.ConnectionReset)
				Dispatcher.UIThread.Invoke(() => _notificationService.Show("Уведомление!", "Подключение прервано", NotificationType.Error));
		}
		catch (HttpRequestException ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "HttpError", this, $"|Requests|Message: {ex.Message}\nStatus code: {ex.StatusCode}, RequestError: {ex.HttpRequestError}");
			Dispatcher.UIThread.Invoke(() => _notificationService.Show("Уведомление!", ex.Message, NotificationType.Error));
		}
		catch (Exception ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "Error", this, "Message: " + ex.Message + "\nsource: " + ex.GetType().FullName);
		}
	}

	async Task FillAnalyses()
	{
		try
		{
			_notificationService.Show("Уведомление", "Загрузка списка анализов");
			using var response = await APIService.For<IAnalysis>().GetAnalyses().ConfigureAwait(false);
			if (!response?.IsSuccessful ?? false)
				return;

			AllAnalyses = [.. response.Content];
			SelectedAnalysis = AllAnalyses[0];
		}
		catch (HttpRequestException ex) when (ex.GetBaseException() is SocketException sex && ex.InnerException is IOException ioex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "SHttpError", this, $"|Analyses|Message: {sex.Message}\n, RequestError: {sex.SocketErrorCode}");
			if (sex.SocketErrorCode == SocketError.ConnectionReset)
				Dispatcher.UIThread.Invoke(() => _notificationService.Show("Уведомление!", ioex.Message, NotificationType.Error));
		}
		catch (HttpRequestException ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "HttpError", this, $"|Analyses|Message: {ex.Message}\n\t\tStatus code: {ex.StatusCode}, RequestError: {ex.HttpRequestError}");
			Dispatcher.UIThread.Invoke(() => _notificationService.Show("Уведомление!", ex.Message, NotificationType.Error));
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
	[NotifyPropertyChangedFor(nameof(AnalysesInRecordTotalPrice))]
	private ObservableCollection<Analysis> _analysesInRecord = [];

	[ObservableProperty]
	private AnalysisOrder _orderRecord = new();

	[ObservableProperty]
	public decimal _analysesInRecordTotalPrice = 0;

	[ObservableProperty]
	private DateTimeOffset? _selectedDate = null;
	[ObservableProperty]
	private TimeSpan? _selectedTime = null;


	#region Address
	[ObservableProperty]
	private string _address = string.Empty;

	[ObservableProperty]
	private string _room = string.Empty;

	[ObservableProperty]
	private string _entrance = string.Empty;

	[ObservableProperty]
	private string _floor = string.Empty;

	[ObservableProperty]
	private string? _intercome = null;
	#endregion Address


	[RelayCommand]
	private async Task NewOrder()
	{
		if(SelectedDate == null || SelectedTime == null || Address.IsNullOrWhiteSpace() || AnalysesInRecord.Count == 0)
		{
			_notificationService.Show("Ошибка", "Поля 'Адрес', 'Дата и время', 'Анализы' не могут быть пустыми", NotificationType.Error);
			return;
		}

		IsRecordButtonEnabled = false;

		var analysisDateTime = new DateTime(DateOnly.FromDateTime(SelectedDate.Value.Date), new TimeOnly(SelectedTime!.Value.Hours, SelectedTime.Value.Minutes));

		try
		{
			using var ordersResponse = await APIService.For<IAnalysisOrder>().GetAnalysisOrders().ConfigureAwait(false);
			if (ordersResponse.Content!.Any(x =>
				x.AnalysisDatetime == analysisDateTime &&
				(x.UserId == _appService.CurrentUser!.Id || x.PatientId == SelectedPatient!.Id)
			))
			{
				_notificationService.Show("Ошибка", "На текущее время уже имеется запись");
				IsRecordButtonEnabled = true;
				return;
			}
		} catch(ArgumentNullException ex)
		{
			_notificationService.Show("Ошибка", $"Message: {ex.Message}, Param Name: {ex.ParamName}");
			Logger.Sink.Log(LogEventLevel.Error, "SOMEAREA", this, $"Message: {ex.Message}, Param Name: {ex.ParamName}");
		}

		using var cartResponse = await APIService.For<IPatientAnalysisCart>().Post(new PatientAnalysisCart { PatientId = SelectedPatient!.Id }).ConfigureAwait(false);
		foreach (var item in AnalysesInRecord)
			await APIService.For<IPatientAnalysisCartItem>().Post(new PatientAnalysisCartItem { AnalysisId = item.Id, PatientAnalysisCartId = cartResponse.Content!.Id });
		using var orderResponse = await APIService.For<IAnalysisOrder>().Post(new AnalysisOrder
		{
			PatientAnalysisCart = cartResponse.Content,
			PatientAnalysisCartId = cartResponse.Content!.Id,

			Patient = SelectedPatient,
			PatientId = SelectedPatient.Id,

			User = _appService.CurrentUser!,
			UserId = _appService.CurrentUser!.Id,

			Comment = OrderRecord.Comment,

			RegistrationDate = DateTime.Now,
			AnalysisDatetime = analysisDateTime
		}).ConfigureAwait(false);

		if (orderResponse.IsSuccessful)
		{
			try
			{
				_notificationService.Show("Успех!", $"Пациент успешно записан на {analysisDateTime:dd.MM.yyyy HH:mm}");
				if (!SelectedPatient.Email.IsNullOrWhiteSpace())
					_notificationService.Show("Уведомление", "Также при подключении к интернету пациенту могло прийти сообщение на почту");
			}
			catch (Exception ex)
			{

			}
		}
		else
		{
			_notificationService.Show("Ошибка!", $"Возникли некоторые ошибки: {orderResponse.Error.Content}");
			Logger.Sink!.Log(LogEventLevel.Error, "NewOrder", this, $"Message: {orderResponse.Error.Content}");
		}

		Address = string.Empty;
		Intercome = string.Empty;
		Floor = string.Empty;
		Entrance = string.Empty;
		Room = string.Empty;
		AnalysesInRecord.Clear();
		SelectedDate = null;
		SelectedTime = null;

		OrderRecord = new();

		IsRecordButtonEnabled = true;
	}

	[RelayCommand]
	private void AddAnalysis2Record()
	{
		if (AnalysesInRecord.Contains(SelectedAnalysis!))
			_notificationService.Show("Уведомление", "Список уже содержит текущий анализ");
		if(SelectedAnalysis != null && !AnalysesInRecord.Contains(SelectedAnalysis))
			AnalysesInRecord.Add(SelectedAnalysis);
	}

	[RelayCommand]
	private void DeleteAnalysis(Analysis analysis) =>
		AnalysesInRecord.Remove(analysis);

	#endregion Record
}