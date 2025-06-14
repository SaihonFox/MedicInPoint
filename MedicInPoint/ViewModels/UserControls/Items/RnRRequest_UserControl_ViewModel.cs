using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.API.SignalR;
using MedicInPoint.Models;
using MedicInPoint.Services;

using Microsoft.AspNetCore.SignalR.Client;

namespace MedicInPoint.ViewModels.UserControls.Items;

public partial class RnRRequest_UserControl_ViewModel() : ViewModelBase
{
	public required Func<Request, Task<Request?>> OnAcceptRequest { get; set; }

	public required Func<Request, Task<Request?>> OnDeclineRequest { get; set; }

	private readonly INotificationService? _notificationService = null;

	public RnRRequest_UserControl_ViewModel(INotificationService notificationService, MedicSignalRConnections connections) : this()
	{
		_notificationService = notificationService;

		connections.RequestConnection.On<Request>("RequestUpdated", request => Request = request);
	}

	[ObservableProperty]
	private Request? _request = null;

	[ObservableProperty]
	private bool _isButtonEnabled = true;

	[RelayCommand]
	private async Task AcceptRequest()
	{
		IsButtonEnabled = false;
		_notificationService!.Show("Запрос", "Одобрение");
		var request = await OnAcceptRequest?.Invoke(Request!)!;
		IsButtonEnabled = true;
		if (request == null)
			return;
		
		_notificationService?.Show("Запрос", $"Запрос одобрен");
		Request = request;
	}

	[RelayCommand]
	private async Task DeclineRequest()
	{
		IsButtonEnabled = false;
		_notificationService!.Show("Запрос", "Отклонение");
		var request = await OnDeclineRequest?.Invoke(Request!)!;
		IsButtonEnabled = true;
		if (request == null)
			return;

		_notificationService.Show("Запрос", "Запрос отклонен");
		Request = request;
	}
}