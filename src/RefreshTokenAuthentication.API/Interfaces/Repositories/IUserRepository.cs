﻿using RefreshTokenAuthentication.API.Arguments;
using RefreshTokenAuthentication.API.Entities;
using System.Linq.Expressions;

namespace RefreshTokenAuthentication.API.Interfaces.Repositories;

public interface IUserRepository
{
    Task CreateAsync(User user);
    Task<string?> GetAuthenticationTokenAsync(User user);
    Task<User?> GetByPredicateAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken);
    Task<bool> LoginAsync(LoginArgument loginArgument);
    Task RemoveAuthenticationTokenAsync(User user);
    Task SetAuthenticationTokenAsync(User user, string token);
    Task<bool> UserNameExistsAsync(string userName, CancellationToken cancellationToken);
}
