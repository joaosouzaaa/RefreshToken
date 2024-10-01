using RefreshTokenAuthentication.API.Data.Repositories;
using RefreshTokenAuthentication.API.Interfaces.Repositories;

namespace RefreshTokenAuthentication.API.DependencyInjection;

internal static class RepositoriesDependencyInjection
{
    internal static void AddRepositoriesDependencyInjection(this IServiceCollection services)
    {
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}
