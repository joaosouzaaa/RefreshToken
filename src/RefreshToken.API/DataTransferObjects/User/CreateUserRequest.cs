namespace RefreshToken.API.DataTransferObjects.User;

public sealed record CreateUserRequest(
    string Email,
    string Password);
