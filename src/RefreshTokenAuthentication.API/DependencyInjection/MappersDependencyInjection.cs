using RefreshTokenAuthentication.API.Interfaces.Mappers;
using RefreshTokenAuthentication.API.Mappers;

namespace RefreshTokenAuthentication.API.DependencyInjection;

internal static class MappersDependencyInjection
{
    internal static void AddMappersDependencyInjection(this IServiceCollection services) =>
        services.AddScoped<IUserMapper, UserMapper>();
}
