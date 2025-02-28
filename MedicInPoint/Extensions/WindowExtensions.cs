using Avalonia.Controls.Notifications;
using Avalonia.Controls;

namespace MedicInPoint.Extensions;

public static class WindowExtensions
{
	public static WindowNotificationManager ShowNotification(this Window window, string title, string text, NotificationType type = NotificationType.Information) =>
		new WindowNotificationManager(window) { Position = NotificationPosition.BottomRight, MaxItems = 3 }
			.Also(wnm => wnm.Show(new Notification(title ?? "", text ?? "", type)));

	public static WindowNotificationManager ErrorNotification(this Window window, string title, string text) => window.ShowNotification(title ?? "", text ?? "", NotificationType.Error);
}