using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RefreshToken.API.Entities;

namespace RefreshToken.API.Data.DatabaseContexts;

public sealed class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
}
