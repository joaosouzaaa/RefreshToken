using Microsoft.EntityFrameworkCore;
using RefreshTokenAuthentication.API.Data.DatabaseContexts;
using RefreshTokenAuthentication.API.Entities;
using RefreshTokenAuthentication.API.Interfaces.Repositories;
using System.Linq.Expressions;

namespace RefreshTokenAuthentication.API.Data.Repositories;

public sealed class RefreshTokenRepository(ApplicationDbContext dbContext) : IRefreshTokenRepository, IDisposable
{
    private DbSet<RefreshToken> DbContextSet => dbContext.Set<RefreshToken>();

    public Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        DbContextSet.Add(refreshToken);

        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<RefreshToken?> GetByPredicateAsync(
        Expression<Func<RefreshToken, bool>> predicate,
        CancellationToken cancellationToken) =>
        DbContextSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

    public Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
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
