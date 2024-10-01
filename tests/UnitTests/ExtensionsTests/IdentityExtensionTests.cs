using RefreshTokenAuthentication.API.Extensions;
using System.Security.Claims;
using System.Security.Principal;

namespace UnitTests.ExtensionsTests;

public sealed class IdentityExtensionTests
{
    [Fact]
    public void GetUserId_SuccessfulScenario_ReturnsUserId()
    {
        // A
        const string nameIdentifier = "test";
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, nameIdentifier)
        ],
        "mock");

        // A 
        var userId = identity.GetUserId();

        // A 
        Assert.Equal(nameIdentifier, userId);
    }

    [Fact]
    public void GetUserId_WithNullIdentity_ReturnsNull()
    {
        // A 
        IIdentity? identity = null;

        // A
        var userId = identity?.GetUserId();

        // A
        Assert.Null(userId);
    }

    [Fact]
    public void GetUserId_WithUnauthenticatedIdentity_ReturnsNull()
    {
        // A
        var identity = new ClaimsIdentity();

        // A 
        var userId = identity.GetUserId();

        // A 
        Assert.Null(userId);
    }
}
