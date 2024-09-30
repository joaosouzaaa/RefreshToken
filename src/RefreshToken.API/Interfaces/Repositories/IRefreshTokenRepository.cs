using RefreshToken.API.Entities;
using System.Linq.Expressions;

namespace RefreshToken.API.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(ApplicationRefreshToken refreshToken, CancellationToken cancellationToken);
    Task<ApplicationRefreshToken?> GetByPredicateAsync(Expression<Func<ApplicationRefreshToken, bool>> predicate, CancellationToken cancellationToken);
    Task UpdateAsync(ApplicationRefreshToken refreshToken, CancellationToken cancellationToken);
}
