using RefreshToken.API.Settings.NotificationSettings;

namespace RefreshToken.API.Interfaces.Settings;

public interface INotificationHandler
{
    void AddNotification(string key, string message);
    List<Notification> GetNotifications();
    bool HasNotifications();
}
