using Microsoft.Extensions.Options;
using RefreshToken.API.Options;
using RefreshToken.API.Services;
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
}
