using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace UnitTests.TestBuilders;

public static class TokenValidatedContextBuilder
{
    public static TokenValidatedContext BuildTokenValidatedContext(JwtSecurityToken? token)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        return new(
            new DefaultHttpContext(),
            new AuthenticationScheme("TestScheme", null, typeof(JwtBearerHandler)),
            new JwtBearerOptions())
        {
            Principal = claimsPrincipal,
            SecurityToken = token!
        };
    }
}
