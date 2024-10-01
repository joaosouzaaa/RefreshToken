using Microsoft.AspNetCore.Authentication.JwtBearer;
using RefreshTokenAuthentication.API.Interfaces.Repositories;
using System.Security.Claims;

namespace RefreshTokenAuthentication.API.Events;

public sealed class TokenRevokedBearerEvents(IUserRepository userRepository) : JwtBearerEvents
{
    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var token = context.SecurityToken;

        if (token is null)
        {
            return;
        }

        var tokenValue = token.UnsafeToString();

        var claimsIdentity = context.Principal!.Claims;
        var userId = claimsIdentity.First(a => a.Type == ClaimTypes.NameIdentifier)!.Value;

        var user = await userRepository.GetByPredicateAsync(u => u.Id == userId, context.HttpContext.RequestAborted);

        if (user is not null)
        {
            var authenticationToken = await userRepository.GetAuthenticationTokenAsync(user);

            if (authenticationToken != tokenValue)
            {
                context.Fail("Token invalid.");
            }
        }
        else
        {
            context.Fail("User does not exist");
        }
    }
}
