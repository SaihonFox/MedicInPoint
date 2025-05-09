using Avalonia.Controls.Notifications;

namespace MedicInPoint.Services;

public interface INotificationService
{
	Notification Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null);

	void CloseAll();

	void Close(INotification notification);
}