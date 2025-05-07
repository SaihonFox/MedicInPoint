using Avalonia.Controls.Notifications;
using Avalonia.Threading;

namespace MedicInPoint.Services;

public class NotificationService(INotificationManager manager) : INotificationService
{
	private readonly INotificationManager _manager = manager;

	public Notification Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null)
	{
		var notification = new Notification(title, message, type, expiration);
		Dispatcher.UIThread.Invoke(() => _manager.Show(notification));
		return notification;
	}
}