using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RefreshToken.API.Options;
using RefreshToken.API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UnitTests.TestBuilders;

namespace UnitTests.ServicesTests;

public sealed class JwtServiceTests
{
    private readonly IOptions<TokenOptions> _tokenOptions;
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        _tokenOptions = Options.Create(new TokenOptions()
        {
            Audience = "test",
            ExpirationTimeInMinutes = 123,
            Issuer = "test",
            Key = "fea6c242ae2ef3b6fa464d2f66d4754b"
        });
        _jwtService = new JwtService(_tokenOptions);
    }

    [Fact]
    public void GenerateToken_SuccessfulScenario_ReturnsToken()
    {
        // A
        var user = UserBuilder.NewObject().DomainBuild();

        // A
        var tokenResult = _jwtService.GenerateToken(user);

        // A
        Assert.False(string.IsNullOrEmpty(tokenResult));
    }

    [Fact]
    public async Task GetNameIdentifierFromTokenAsync_SuccessfulScenario_ReturnsNameIdentifier()
    {
        // A
        const string expectedNameIdentifier = "3d61ea5520d5414fac7ef76782548d36";

        var tokenOptions = _tokenOptions.Value;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(tokenOptions.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, expectedNameIdentifier)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(30),
            Issuer = tokenOptions.Issuer,
            Audience = tokenOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwtToken = tokenHandler.WriteToken(token);

        // A
        var nameIdentifierResult = await _jwtService.GetNameIdentifierFromTokenAsync(jwtToken);

        // A
        Assert.Equal(expectedNameIdentifier, nameIdentifierResult);
    }

    [Theory]
    [InlineData("")]
    [InlineData("test")]
    [InlineData(null)]
    public async Task GetNameIdentifierFromTokenAsync_InvalidToken_ReturnsNull(string? invalidToken)
    {
        // A
        var nameIdentifierResult = await _jwtService.GetNameIdentifierFromTokenAsync(invalidToken!);

        // A
        Assert.Null(nameIdentifierResult);
    }
}
