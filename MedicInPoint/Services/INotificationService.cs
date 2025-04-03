using Avalonia.Controls.Notifications;

namespace MedicInPoint.Services;

public interface INotificationService
{
	void Show(string title, string message, NotificationType type = NotificationType.Information);
}