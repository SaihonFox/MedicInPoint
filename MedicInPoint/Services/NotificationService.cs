using Avalonia.Controls.Notifications;

namespace MedicInPoint.Services;

public class NotificationService(INotificationManager manager) : INotificationService
{
	private readonly INotificationManager _manager = manager;

	public void Show(string title, string message, NotificationType type = NotificationType.Information)
	{
		var notification = new Notification(title, message, type);
		_manager.Show(notification);
	}
}