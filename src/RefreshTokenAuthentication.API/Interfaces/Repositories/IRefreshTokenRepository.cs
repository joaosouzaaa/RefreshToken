using RefreshTokenAuthentication.API.Entities;
using System.Linq.Expressions;

namespace RefreshTokenAuthentication.API.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task<RefreshToken?> GetByPredicateAsync(Expression<Func<RefreshToken, bool>> predicate, CancellationToken cancellationToken);
    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
}
