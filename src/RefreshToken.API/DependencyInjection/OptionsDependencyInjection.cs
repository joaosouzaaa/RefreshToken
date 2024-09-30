using Microsoft.AspNetCore.Identity;
using RefreshToken.API.Constants;

namespace RefreshToken.API.DependencyInjection;

internal static class OptionsDependencyInjection
{
    internal static void AddOptionsDependencyInjection(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<TokenOptions>(configuration.GetSection(OptionsConstants.TokenSection));
}
