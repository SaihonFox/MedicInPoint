using Avalonia.Controls;
using Avalonia.Input;

using MedicInPoint.API.AIMLAPI.Models;
using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.API.SignalR;
using MedicInPoint.Models;
using MedicInPoint.Services;
using MedicInPoint.ViewModels.Pages.Doctor;
using MedicInPoint.ViewModels.UserControls.Items;
using MedicInPoint.Views.UserControls.Items;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace MedicInPoint.Views.Pages.Doctor;

public partial class RnRDoctorView : UserControl
{
	public RnRDoctorViewModel ViewModel { get; private set; } = null!;

	private readonly MedicSignalRConnections connections = App.services.GetRequiredService<MedicSignalRConnections>();

	public RnRDoctorView()
	{
		Loaded += (s, e) => ViewModel = (RnRDoctorViewModel)DataContext!;

		InitializeComponent();

		if (!Design.IsDesignMode)
			FillRequests();

		connections.RequestConnection.On<Request>("RequestAdded", request => requests_ic.Items.Insert(0, request));

		datepicker.MinYear = DateTimeOffset.Now;
		datepicker.MaxYear = DateTimeOffset.Now.AddMonths(6);

		patients_lb.SelectionChanged += Patients_lb_SelectionChanged;
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
		foreach (var request in response.Content!.Reverse<Request>().Where(x => x.PatientId == (patients_lb.SelectedItem as Patient)?.Id))
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
	}

	async Task<Request?> RequestAccepted(Request request)
	{
		request.RequestStateId = 3;
		request.RequestChanged = DateTime.Now;
		using var response = await APIService.For<IRequest>().PutRequest(request);
		if (!response.IsSuccessful)
			return null;
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
}