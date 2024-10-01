using RefreshTokenAuthentication.API.Entities;

namespace UnitTests.TestBuilders;

public sealed class RefreshTokenBuilder
{
    private DateTime _expiryDate = DateTime.UtcNow;

    public static RefreshTokenBuilder NewObject() =>
        new();

    public RefreshToken DomainBuild() =>
        new()
        {
            ExpiryDate = _expiryDate,
            Id = Guid.NewGuid(),
            UserId = "test",
            Value = "asdasd"
        };

    public RefreshTokenBuilder WithExpiryDate(DateTime expiryDate)
    {
        _expiryDate = expiryDate;

        return this;
    }
}
