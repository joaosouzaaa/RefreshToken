namespace RefreshTokenAuthentication.API.Options;

public sealed class TokenOptions
{
    public required string Key { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required double ExpirationTimeInMinutes { get; init; }
    public double RefreshTokenExpirationTimeInMinutes { get; init; }
}
