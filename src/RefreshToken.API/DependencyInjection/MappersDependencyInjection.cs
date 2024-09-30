using RefreshToken.API.Interfaces.Mappers;
using RefreshToken.API.Mappers;

namespace RefreshToken.API.DependencyInjection;

internal static class MappersDependencyInjection
{
    internal static void AddMappersDependencyInjection(this IServiceCollection services) =>
        services.AddScoped<IUserMapper, UserMapper>();
}
