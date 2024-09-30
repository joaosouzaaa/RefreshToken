using Microsoft.EntityFrameworkCore;
using RefreshToken.API.Data.DatabaseContexts;
using RefreshToken.API.Entities;
using RefreshToken.API.Interfaces.Repositories;
using System.Linq.Expressions;

namespace RefreshToken.API.Data.Repositories;

public sealed class RefreshTokenRepository(ApplicationDbContext dbContext) : IRefreshTokenRepository, IDisposable
{
    private DbSet<ApplicationRefreshToken> DbContextSet => dbContext.Set<ApplicationRefreshToken>();

    public Task AddAsync(ApplicationRefreshToken refreshToken, CancellationToken cancellationToken)
    {
        DbContextSet.Add(refreshToken);

        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<ApplicationRefreshToken?> GetByPredicateAsync(
        Expression<Func<ApplicationRefreshToken, bool>> predicate,
        CancellationToken cancellationToken) =>
        DbContextSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

    public Task UpdateAsync(ApplicationRefreshToken refreshToken, CancellationToken cancellationToken)
    {
        DbContextSet.Update(refreshToken);

        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        dbContext.Dispose();

        GC.SuppressFinalize(this);
    }
}
