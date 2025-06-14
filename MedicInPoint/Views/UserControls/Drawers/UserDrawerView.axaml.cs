using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Logging;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.Services;
using MedicInPoint.ViewModels.UserControls.Drawers;

using Microsoft.Extensions.DependencyInjection;

namespace MedicInPoint.Views.UserControls.Drawers;

public partial class UserDrawerView : UserControl
{
	public UserDrawerViewModel ViewModel => DataContext as UserDrawerViewModel;

	public readonly INotificationService notificationService = null!;

	public UserDrawerView()
	{
		InitializeComponent();

		notificationService = App.services.GetRequiredService<INotificationService>();

		fire_user.Click += Fire_user_Click;
		return_user.Click += Return_user_Click;
	}

	async void Fire_user_Click(object? sender, RoutedEventArgs e)
	{
		if (App.services.GetRequiredService<IAppStateService>().CurrentUser!.Id == ViewModel.User!.Id)
		{
			notificationService.Show("Ошибка!", "Вы не можете уволить самого себя");
			return;
		}

		var notification = notificationService.Show("Уведомление", $"Увольнение {ViewModel.User.FullName}");
		using var response = await APIService.For<IUser>().ChangeBlockStatus(ViewModel.User!.Id, true);
		if (!response.IsSuccessful)
		{
			notificationService.Close(notification);
			notificationService.Show("Ошибка!", $"Ошибка: {response.Error.Message}", NotificationType.Error);
			Logger.Sink!.Log(LogEventLevel.Error, "UserDrawerFire", this, $"Ошибка: {response.Error.Message}, StatusCode: {response.Error.StatusCode}");
			return;
		}
		notificationService.Close(notification);

		notificationService.Show("Успех!", $"Вы успешно уволили {ViewModel.User.FullName}", NotificationType.Success);
		ViewModel.User = response.Content;
	}

	async void Return_user_Click(object? sender, RoutedEventArgs e)
	{
		if (App.services.GetRequiredService<IAppStateService>().CurrentUser!.Id == ViewModel.User!.Id)
		{
			notificationService.Show("Ошибка!", "Вы не можете вернуть самого себя");
			return;
		}

		var notification = notificationService.Show("Уведомление", $"Возвращение {ViewModel.User.FullName}");
		using var response = await APIService.For<IUser>().ChangeBlockStatus(ViewModel.User!.Id, false);
		if (!response.IsSuccessful)
		{
			notificationService.Close(notification);
			notificationService.Show("Ошибка!", $"Ошибка: {response.Error.Message}", NotificationType.Error);
			Logger.Sink!.Log(LogEventLevel.Error, "UserDrawerFire", this, $"Ошибка: {response.Error.Message}, StatusCode: {response.Error.StatusCode}");
			return;
		}
		notificationService.Close(notification);

		notificationService.Show("Успех!", $"Вы успешно вернули {ViewModel.User.FullName}", NotificationType.Success);
		ViewModel.User = response.Content;
	}
}