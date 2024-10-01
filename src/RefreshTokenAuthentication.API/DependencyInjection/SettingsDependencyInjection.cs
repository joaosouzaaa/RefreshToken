using FluentValidation;
using RefreshTokenAuthentication.API.Filters;
using RefreshTokenAuthentication.API.Interfaces.Settings;
using RefreshTokenAuthentication.API.Settings.NotificationSettings;
using System.Reflection;

namespace RefreshTokenAuthentication.API.DependencyInjection;

internal static class SettingsDependencyInjection
{
    internal static void AddSettingsDependencyInjection(this IServiceCollection services)
    {
        services.AddScoped<INotificationHandler, NotificationHandler>();
        services.AddScoped<NotificationFilter>();

        services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(Program)));
    }
}
