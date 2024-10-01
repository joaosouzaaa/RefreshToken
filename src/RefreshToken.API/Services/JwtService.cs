using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RefreshToken.API.Entities;
using RefreshToken.API.Interfaces.Services;
using RefreshToken.API.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RefreshToken.API.Services;

public sealed class JwtService(IOptions<TokenOptions> tokenOptions) : IJwtService
{
    private readonly TokenOptions _token = tokenOptions.Value;

    public string GenerateToken(User user)
    {
        var tokenDescription = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(GetUserInitialClaims(user)),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_token.ExpirationTimeInMinutes)),
            Issuer = _token.Issuer,
            Audience = _token.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_token.Key)),
                SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescription);

        return tokenHandler.WriteToken(token);
    }

    public async Task<string?> GetNameIdentifierFromTokenAsync(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_token.Key)),
            ValidateIssuer = true,
            ValidIssuer = _token.Issuer,
            ValidateAudience = true,
            ValidAudience = _token.Audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenValidationResult = await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);

        if (!tokenValidationResult.IsValid)
        {
            return null;
        }

        return tokenValidationResult.Claims.FirstOrDefault(c => c.Key == ClaimTypes.NameIdentifier).Value.ToString();
    }

    private static List<Claim> GetUserInitialClaims(User user) =>
        [
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!)
        ];
}
