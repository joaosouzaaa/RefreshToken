namespace RefreshToken.API.DataTransferObjects.RefreshToken;

public sealed record AddOrUpdateRefreshTokenResponse(
    string Value,
    DateTime ExpiryDate);
