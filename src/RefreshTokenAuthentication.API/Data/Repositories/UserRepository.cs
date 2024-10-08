﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using RefreshTokenAuthentication.API.Arguments;
using RefreshTokenAuthentication.API.Data.DatabaseContexts;
using RefreshTokenAuthentication.API.Entities;
using RefreshTokenAuthentication.API.Interfaces.Repositories;
using System.Linq.Expressions;

namespace RefreshTokenAuthentication.API.Data.Repositories;

public sealed class UserRepository(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ApplicationDbContext dbContext)
    : IUserRepository,
    IDisposable
{
    public Task CreateAsync(User user) =>
        userManager.CreateAsync(user, user.PasswordHash!);

    public Task<string?> GetAuthenticationTokenAsync(User user) =>
        userManager.GetAuthenticationTokenAsync(user, JwtBearerDefaults.AuthenticationScheme, JwtConstants.TokenType);

    public Task<User?> GetByPredicateAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken) =>
        dbContext.Users.FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<bool> LoginAsync(LoginArgument loginArgument)
    {
        var signInResult = await signInManager.PasswordSignInAsync(loginArgument.Email, loginArgument.Password, false, false);

        return signInResult.Succeeded;
    }

    public Task RemoveAuthenticationTokenAsync(User user) =>
        userManager.RemoveAuthenticationTokenAsync(user, JwtBearerDefaults.AuthenticationScheme, JwtConstants.TokenType);

    public Task SetAuthenticationTokenAsync(User user, string token) =>
        userManager.SetAuthenticationTokenAsync(user, JwtBearerDefaults.AuthenticationScheme, JwtConstants.TokenType, token);

    public Task<bool> UserNameExistsAsync(string userName, CancellationToken cancellationToken) =>
        dbContext.Users.AnyAsync(u => u.UserName == userName, cancellationToken);

    public void Dispose()
    {
        dbContext.Dispose();
        userManager.Dispose();

        GC.SuppressFinalize(this);
    }
}
