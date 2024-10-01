using Microsoft.AspNetCore.Identity;
using RefreshTokenAuthentication.API.Constants;

namespace RefreshTokenAuthentication.API.DependencyInjection;

internal static class OptionsDependencyInjection
{
    internal static void AddOptionsDependencyInjection(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<TokenOptions>(configuration.GetSection(OptionsConstants.TokenSection));
}
