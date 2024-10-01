using RefreshTokenAuthentication.API.Interfaces.Services;
using RefreshTokenAuthentication.API.Services;

namespace RefreshTokenAuthentication.API.DependencyInjection;

internal static class ServicesDependencyInjection
{
    internal static void AddServicesDependencyInjection(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUserService, UserService>();
    }
}
