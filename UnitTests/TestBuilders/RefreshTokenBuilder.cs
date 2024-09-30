using RefreshToken.API.Entities;

namespace UnitTests.TestBuilders;

public sealed class RefreshTokenBuilder
{
    public static RefreshTokenBuilder NewObject() =>
        new();

    public ApplicationRefreshToken DomainBuild() =>
        new()
        {
            ExpiryDate = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            UserId = "test",
            Value = "asdasd"
        };
}
