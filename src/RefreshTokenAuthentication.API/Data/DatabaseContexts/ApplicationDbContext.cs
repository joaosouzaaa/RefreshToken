using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RefreshTokenAuthentication.API.Entities;

namespace RefreshTokenAuthentication.API.Data.DatabaseContexts;

public sealed class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
