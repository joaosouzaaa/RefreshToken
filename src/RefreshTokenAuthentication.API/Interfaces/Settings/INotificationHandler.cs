using RefreshTokenAuthentication.API.Settings.NotificationSettings;

namespace RefreshTokenAuthentication.API.Interfaces.Settings;

public interface INotificationHandler
{
    void AddNotification(string key, string message);
    List<Notification> GetNotifications();
    bool HasNotifications();
}
