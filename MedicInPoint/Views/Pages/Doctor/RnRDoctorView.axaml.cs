using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Logging;
using Avalonia.Xaml.Interactivity;

using Medic.API.Refit.Placeholders;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.API.SignalR;
using MedicInPoint.Extensions;
using MedicInPoint.Models;
using MedicInPoint.Services;
using MedicInPoint.ViewModels.Pages.Doctor;
using MedicInPoint.ViewModels.UserControls.Items;
using MedicInPoint.Views.UserControls.Items;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

using MIP.LocalDB;

namespace MedicInPoint.Views.Pages.Doctor;

public partial class RnRDoctorView : UserControl
{
	public RnRDoctorViewModel ViewModel { get; private set; } = null!;

	private readonly MedicSignalRConnections connections = App.services.GetRequiredService<MedicSignalRConnections>();
	private readonly INotificationService notification = App.services.GetRequiredService<INotificationService>();
	private readonly IAppStateService _appService = App.services.GetRequiredService<IAppStateService>();

	public RnRDoctorView()
	{
		Loaded += (s, e) => ViewModel = (RnRDoctorViewModel)DataContext!;

		InitializeComponent();

		var collection = new BehaviorCollection
		{
			new TestAutoCompleteBehavior.Behaviors.AutoCompleteZeroMinimumPrefixLengthDropdownBehavior()
		};
		Interaction.SetBehaviors(acb, collection);

		using var context = new LocalDbContext();
		var names = context.RnRDoctorSearches.ToList().Select(aas => aas.name);
		acb.ItemsSource = names.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();

		acb.KeyDown += acb_KeyDown;

		if (!Design.IsDesignMode)
			FillRequests();

		connections.RequestConnection.On<Request>("RequestAdded", request => requests_ic.Items.Insert(0, request));

		datepicker.MinYear = DateTimeOffset.Now;
		datepicker.MaxYear = DateTimeOffset.Now.AddMonths(6);

		void fixTime()
		{
			if (datepicker.SelectedDate.HasValue && timepicker.SelectedValue != null)
			{
				if (DateTime.Now.TimeOfDay > TimeSpan.FromHours(20))
					datepicker.SelectedDate = datepicker.SelectedDate.Value.AddDays(1);

				var checkTime = new TimeSpan((int)timepicker.SelectedValue, 0, 0);
				var currentTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

				if (datepicker.SelectedDate.Value.Date == DateTime.Today &&
					checkTime <= currentTime
				)
				{
					var nextTime = checkTime.Add(TimeSpan.FromHours(1));
					while (nextTime <= currentTime)
						nextTime = nextTime.Add(TimeSpan.FromHours(1));

					timepicker.SelectedValue = nextTime.Hours;
				}
			}
		}

		timepicker.SelectionChanged += (_, e) =>
		{
			if (timepicker.SelectedItem == null)
				return;

			if (new TimeSpan((int)timepicker.SelectedItem, 0, 0) < TimeSpan.FromHours(8))
				timepicker.SelectedItem = TimeSpan.FromHours(8).Hours;
			if (new TimeSpan((int)timepicker.SelectedItem, 0, 0) > TimeSpan.FromHours(20))
				timepicker.SelectedItem = TimeSpan.FromHours(20).Hours;
			fixTime();
		};
		datepicker.SelectedDateChanged += (_, e) =>
		{
			fixTime();
		};
		if (OperatingSystem.IsWindows())
		{
			SystemEvents.TimeChanged += (_, e) =>
			{
				fixTime();
			};
		}

		patients_lb.SelectionChanged += Patients_lb_SelectionChanged;
	}

	async void acb_KeyDown(object? source, KeyEventArgs e)
	{
		using var context = new LocalDbContext();
		if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(acb.SearchText))
		{
			var list = await context.RnRDoctorSearches.ToListAsync();
			if (list.FirstOrDefault(a => a.name == acb.SearchText) != null || acb.SearchText.IsNullOrWhiteSpace())
				return;

			context.RnRDoctorSearches.Add(new() { name = acb.SearchText });
			await context.SaveChangesAsync();

			var enumerable = acb.ItemsSource!.Cast<string>().ToList();
			enumerable.Add(acb.SearchText);
			enumerable = [.. enumerable.Distinct()];
			acb.ItemsSource = enumerable;
		}
		await context.DisposeAsync();
	}

	private void Patients_lb_SelectionChanged(object? sender, SelectionChangedEventArgs e) =>
		FillRequests();

	void TB_Int_Mask(object? source, KeyEventArgs e)
	{
		if (!(e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key == Key.Back || e.Key == Key.Delete))
			e.Handled = true;
	}

	async void FillRequests()
	{
		using var response = await APIService.For<IRequest>().GetRequests();
		if (!response.IsSuccessful)
			return;

		requests_ic.Items.Clear();
		foreach (var request in response.Content!.Reverse<Request>().Where(x => x.DoctorId == _appService.CurrentUser.Id && x.PatientId == (patients_lb.SelectedItem as Patient)?.Id))
		{
			requests_ic.Items.Add(new RnRRequest_UserControl_View
			{
				DataContext = new RnRRequest_UserControl_ViewModel(App.services.GetRequiredService<INotificationService>(), connections)
				{
					Request = request,
					OnAcceptRequest = RequestAccepted,
					OnDeclineRequest = RequestDeclined
				}
			});
		}
		centerText.IsVisible = requests_ic.Items.Count == 0;
	}

	async Task<Request?> RequestAccepted(Request request)
	{
		request.RequestStateId = 3;
		request.RequestChanged = DateTime.Now;
		using var response = await APIService.For<IRequest>().PutRequest(request);
		if (!response.IsSuccessful)
			return null;

		Request2Order(request);

		return response.Content!;
	}

	async Task<Request?> RequestDeclined(Request request)
	{
		request.RequestStateId = 2;
		request.RequestChanged = DateTime.Now;
		using var response = await APIService.For<IRequest>().PutRequest(request);
		if (!response.IsSuccessful)
			return null;
		return response.Content!;
	}

	async void Request2Order(Request request)
	{
		using var ordersResponse = await APIService.For<IAnalysisOrder>().GetAnalysisOrders().ConfigureAwait(false);
		if (ordersResponse.Content!.Any(x =>
			x.AnalysisDatetime == request.AnalysisDatetime &&
			(x.UserId == _appService.CurrentUser!.Id || x.PatientId == request.PatientId)
		))
		{
			notification.Show("Ошибка", "На текущее время уже имеется запись");
			return;
		}

		using var cartResponse = await APIService.For<IPatientAnalysisCart>().Post(new PatientAnalysisCart { PatientId = request.PatientId }).ConfigureAwait(false);
		foreach (var item in request.PatientAnalysisCart.PatientAnalysisCartItems)
		{
			Logger.Sink.Log(LogEventLevel.Error, "CART", this, $"Item: {item.Analysis.Id} - {cartResponse.Content.Id}");
			await APIService.For<IPatientAnalysisCartItem>().Post(new PatientAnalysisCartItem { AnalysisId = item.Analysis.Id, PatientAnalysisCartId = cartResponse.Content!.Id });
		}

		using var orderResponse = await APIService.For<IAnalysisOrder>().Post(new AnalysisOrder
		{
			PatientAnalysisCart = cartResponse.Content,
			PatientAnalysisCartId = cartResponse.Content!.Id,

			Patient = request.Patient,
			PatientId = request.PatientId,

			User = request.Doctor!,
			UserId = request.DoctorId,

			Comment = request.Comment,

			RegistrationDate = DateTime.Now,
			AnalysisDatetime = request.AnalysisDatetime
		}).ConfigureAwait(false);

		if (orderResponse.IsSuccessful)
		{
			try
			{
				notification.Show("Успех!", $"Пациент успешно записан на {request.AnalysisDatetime:dd.MM.yyyy HH:mm}");
				if (!request.Patient.Email.IsNullOrWhiteSpace())
					notification.Show("Уведомление", "Также при подключении к интернету пациенту могло прийти сообщение на почту");
			}
			catch (Exception ex)
			{

			}
		}
		else
		{
			notification.Show("Ошибка!", $"Возникли некоторые ошибки: {orderResponse.Error.Content}");
			Logger.Sink!.Log(LogEventLevel.Error, "NewOrder", this, $"Message: {orderResponse.Error.Content}");
		}
	}
}