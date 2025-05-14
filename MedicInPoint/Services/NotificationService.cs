using Avalonia.Controls.Notifications;
using Avalonia.Logging;
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

	public void CloseAll() => Dispatcher.UIThread.Invoke(_manager.CloseAll);

	public void Close(INotification notification) => Dispatcher.UIThread.Invoke(() => _manager.Close(notification));
}