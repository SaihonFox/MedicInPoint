﻿using System.Diagnostics;
using System.Net;

using Avalonia.Controls.Notifications;
using Avalonia.Logging;
using Avalonia.SimpleRouter;
using Avalonia.Threading;

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

		PortCheckJob.PortStateChanged += async (s, e) =>
		{
			if (e.IsPortOpen)
				await Dispatcher.UIThread.Invoke(async() => await FindUser().ConfigureAwait(false)).ConfigureAwait(false);
		};
	}

	[ObservableProperty]
	private string _login = string.Empty;

	[ObservableProperty]
	private string _password = string.Empty;

	[ObservableProperty]
	private bool _isEnterEnabled = true;



	async Task FindUser()
	{
		/*if (Login.IsNullOrWhiteSpace() || Password.IsNullOrWhiteSpace())
			return;

		using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

		try
		{
			using var response = await APIService.For<IUser>().Login(Login.Trim(), Password.Trim(), cts.Token);
			if (!response.IsSuccessful)
				return;

			cts.Cancel();
			var user = response.Content;
			_service.CurrentUser = user;
		}
		catch (ApiException ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "ApiError", this, $"Message: {ex.Message}\nStatus code: {ex.StatusCode}, Uri: {ex.Uri}");
		}
		catch (HttpRequestException ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "HttpError", this, $"Message: {ex.Message}\nStatus code: {ex.StatusCode}, RequestError: {ex.HttpRequestError}");
		}
		catch (TaskCanceledException ex)
		{
			_notificationService.Show("Уведомление", "Ошибка времени: " + ex.Message, NotificationType.Error);
			return;
		}
		catch (TargetInvocationException ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "TargetInvocation", this, "Message: " + ex.TargetSite + ex);
		}
		catch (Exception ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "Error", this, "Message: " + ex.Message + "\nsource: " + ex.GetType().FullName);
		}*/
	}

	async partial void OnLoginChanged(string value) => await FindUser();

	async partial void OnPasswordChanged(string value) => await FindUser();

	[RelayCommand]
	private async Task Enter()
	{
		if (Login.IsNullOrWhiteSpace() || Password.IsNullOrWhiteSpace())
		{
			_notificationService.Show("Уведомление", "Поля не могут быть пустыми");
			return;
		}

		IsEnterEnabled = false;
		if (_service.CurrentUser == null)
		{
			_notificationService.Show("Поиск", "Поиск пользователя с введенными данными");
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			
			try
			{
				using var response = await APIService.For<IUser>().Login(Login.Trim(), Password.Trim());
				if (!response.IsSuccessful)
				{
					if (response.StatusCode == HttpStatusCode.NotFound)
						_notificationService.Show("Уведомление", "Пользователь с такими данными не найден", NotificationType.Warning);
					if (response.StatusCode >= HttpStatusCode.InternalServerError)
						_notificationService.Show("Уведомление", "Ошибка подключения к серверу!", NotificationType.Error);
					IsEnterEnabled = true;
					return;
				}
				stopwatch.Stop();
				_service.CurrentUser = response.Content;
				_notificationService.Show("Поиск", $"Поиск завершен за: {stopwatch.Elapsed.TotalSeconds:0.##} сек.");
			}
			catch (HttpRequestException ex)
			{
				_notificationService.Show("Уведомление", "Ошибка подключения к серверу!", NotificationType.Error);
				Logger.Sink!.Log(LogEventLevel.Error, "HttpError", this, $"Message: {ex.Message}\nStatus code: {ex.StatusCode}, RequestError: {ex.HttpRequestError}");
				return;
			}
			catch (TaskCanceledException ex)
			{
				_notificationService.Show("Уведомление", "Ошибка времени: " + ex.Message, NotificationType.Error);
				Logger.Sink!.Log(LogEventLevel.Error, "TaskError", this, "Message: " + ex.Message + "\nsource: " + ex.GetType().FullName);
				return;
			}
			catch (Exception ex)
			{
				_notificationService.Show("Уведомление", "Ошибка: " + ex.Message, NotificationType.Error);
				Logger.Sink!.Log(LogEventLevel.Error, "Error", this, "Message: " + ex.Message + "\nsource: " + ex.GetType().FullName);
				return;
			}
			finally
			{
				IsEnterEnabled = true;
			}
		}

		_notificationService.Show("Вход", $"Добро пожаловать, {_service.CurrentUser!.FullName}!", NotificationType.Success);
		//await APIService.For<IUserLastEnter>().SetLastEnter(_service.CurrentUser!.Id, DateTime.Now);
		await Task.Delay(1000);

		Login = Password = string.Empty;

		_router.GoTo<MenuViewModel>();

		IsEnterEnabled = true;
	}
}