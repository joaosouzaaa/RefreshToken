namespace RefreshTokenAuthentication.API.DataTransferObjects.User;

public sealed record LoginRequest(
    string Email,
    string Password);
