using Microsoft.EntityFrameworkCore;
using RefreshTokenAuthentication.API.Data.DatabaseContexts;

namespace RefreshTokenAuthentication.API.DependencyInjection;

internal static class MigrationHandler
{
    internal static void MigrateDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            dbContext.Database.Migrate();
        }
        catch
        {
            throw;
        }
    }
}
