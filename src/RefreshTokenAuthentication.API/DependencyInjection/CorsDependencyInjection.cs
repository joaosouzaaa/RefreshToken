using RefreshTokenAuthentication.API.Constants;

namespace RefreshTokenAuthentication.API.DependencyInjection;

internal static class CorsDependencyInjection
{
    internal static void AddCorsDependencyInjection(this IServiceCollection services) =>
        services.AddCors(p => p.AddPolicy(CorsPoliciesNamesConstants.CorsPolicy, builder =>
        {
            builder.AllowAnyMethod()
                   .AllowAnyHeader()
                   .SetIsOriginAllowed(origin => true)
                   .AllowCredentials();
        }));
}
