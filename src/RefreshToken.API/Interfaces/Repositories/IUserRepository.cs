using RefreshToken.API.Arguments;
using RefreshToken.API.Entities;
using System.Linq.Expressions;

namespace RefreshToken.API.Interfaces.Repositories;

public interface IUserRepository
{
    Task CreateAsync(User user);
    Task<User?> GetByPredicateAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken);
    Task<bool> LoginAsync(LoginArgument loginArgument);
    Task SetAuthenticationTokenAsync(User user, string token);
    Task<bool> UserNameExistsAsync(string userName, CancellationToken cancellationToken);
}
