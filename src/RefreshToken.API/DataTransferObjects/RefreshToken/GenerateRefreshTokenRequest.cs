namespace RefreshToken.API.DataTransferObjects.RefreshToken;

public sealed record GenerateRefreshTokenRequest(
    string AccessToken,
    string RefreshToken);
