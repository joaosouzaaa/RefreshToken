namespace RefreshToken.API.DataTransferObjects.User;

public sealed record BearerResponse(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiryDate);
