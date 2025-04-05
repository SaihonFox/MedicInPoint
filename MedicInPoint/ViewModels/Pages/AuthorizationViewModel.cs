using System.Net;

using Avalonia.Controls.Notifications;
using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.Extensions;
using MedicInPoint.Services;

namespace MedicInPoint.ViewModels.Pages;

public partial class AuthorizationViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;
	private readonly INotificationService _notificationService = null!;
	private readonly IAppStateService _service = null!;

	public AuthorizationViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router, INotificationService notificationService, IAppStateService service) : this()
	{
		Title = "Авторизация";
		_router = router;
		_notificationService = notificationService;
		_service = service;
	}

	[ObservableProperty]
	private string _login = string.Empty;

	[ObservableProperty]
	private string _password = string.Empty;

	[ObservableProperty]
	private bool _isEnterEnabled = true;

	async partial void OnLoginChanged(string value)
	{
		if (Login.IsNullOrWhiteSpace() || Password.IsNullOrWhiteSpace())
			return;

		await Task.Run(async () =>
		{
			var response = await APIService.For<IUser>().Login(Login.Trim(), Password.Trim());
			if (!response.IsSuccessStatusCode)
				return;

			var user = response.Content;
			_service.CurrentUser = user;
		});
	}

	async partial void OnPasswordChanged(string value)
	{
		if (Login.IsNullOrWhiteSpace() || Password.IsNullOrWhiteSpace())
			return;

		await Task.Run(async () =>
		{
			var response = await APIService.For<IUser>().Login(Login.Trim(), Password.Trim());
			if (!response.IsSuccessStatusCode)
				return;

			var user = response.Content;
			_service.CurrentUser = user;
		});
	}

	[RelayCommand]
	private async Task Enter()
	{
		if (Login.IsNullOrWhiteSpace() || Password.IsNullOrWhiteSpace())
		{
			_notificationService.Show("Уведомление", "Поля не могут быть пустыми");
			return;
		}

		if (_service.CurrentUser == null)
		{
			IsEnterEnabled = false;

			var response = await APIService.For<IUser>().Login(Login.Trim(), Password.Trim());
			if (!response.IsSuccessStatusCode)
			{
				if(response.StatusCode == HttpStatusCode.NotFound)
					_notificationService.Show("Уведомление", "Пользователь с такими данными не найден");
				if(response.StatusCode >= HttpStatusCode.InternalServerError)
					_notificationService.Show("Уведомление", "Ошибка на стороне сервера!", NotificationType.Warning);
				IsEnterEnabled = true;
				return;
			}
			_service.CurrentUser = response.Content;
		}

		_notificationService.Show("Вход", $"Добро пожаловать, {_service.CurrentUser!.FullName}!", NotificationType.Success);
		await Task.Delay(1000);

		Login = Password = string.Empty;

		_router.GoTo<MenuViewModel>();

		IsEnterEnabled = true;
	}
}