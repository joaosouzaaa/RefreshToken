using FluentValidation;
using RefreshToken.API.Filters;
using RefreshToken.API.Interfaces.Settings;
using RefreshToken.API.Settings.NotificationSettings;
using System.Reflection;

namespace RefreshToken.API.DependencyInjection;

internal static class SettingsDependencyInjection
{
    internal static void AddSettingsDependencyInjection(this IServiceCollection services)
    {
        services.AddScoped<INotificationHandler, NotificationHandler>();
        services.AddScoped<NotificationFilter>();

        services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(Program)));
    }
}
