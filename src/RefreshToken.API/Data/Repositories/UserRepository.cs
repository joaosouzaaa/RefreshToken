using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RefreshToken.API.Arguments;
using RefreshToken.API.Data.DatabaseContexts;
using RefreshToken.API.Entities;
using RefreshToken.API.Interfaces.Repositories;
using System.Linq.Expressions;

namespace RefreshToken.API.Data.Repositories;

public sealed class UserRepository(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ApplicationDbContext dbContext)
    : IUserRepository
{
    public Task CreateAsync(User user) =>
        userManager.CreateAsync(user, user.PasswordHash!);

    public Task<User?> GetByPredicateAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken) =>
        dbContext.Users.FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<bool> LoginAsync(LoginArgument loginArgument)
    {
        var signInResult = await signInManager.PasswordSignInAsync(loginArgument.Email, loginArgument.Password, false, false);

        return signInResult.Succeeded;
    }

    public Task<bool> UserNameExistsAsync(string userName, CancellationToken cancellationToken) =>
        dbContext.Users.AnyAsync(u => u.UserName == userName, cancellationToken);
}
