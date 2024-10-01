using Microsoft.EntityFrameworkCore;
using RefreshTokenAuthentication.API.Data.DatabaseContexts;
using RefreshTokenAuthentication.API.Factories;

namespace RefreshTokenAuthentication.API.DependencyInjection;

internal static class DependencyInjectionHandler
{
    internal static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCorsDependencyInjection();
        services.AddSwaggerDependencyInjection();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString()));

        services.AddIdentityDependencyInjection();
        services.AddOptionsDependencyInjection(configuration);
        services.AddAuthenticationDependencyInjection(configuration);
        services.AddSettingsDependencyInjection();
        services.AddRepositoriesDependencyInjection();
        services.AddMappersDependencyInjection();
        services.AddServicesDependencyInjection();
    }
}
