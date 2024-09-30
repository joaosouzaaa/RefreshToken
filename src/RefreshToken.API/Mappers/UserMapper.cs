using RefreshToken.API.Arguments;
using RefreshToken.API.DataTransferObjects.User;
using RefreshToken.API.Entities;
using RefreshToken.API.Interfaces.Mappers;

namespace RefreshToken.API.Mappers;

public sealed class UserMapper : IUserMapper
{
    public User CreateRequestToDomain(CreateUserRequest createRequest) =>
        new()
        {
            Email = createRequest.Email,
            UserName = createRequest.Email,
            PasswordHash = createRequest.Password
        };

    public GetUserByIdResponse DomainToGetByIdResponse(User user) =>
        new(user.Id,
            user.UserName!);

    public LoginArgument LoginRequestToDomain(LoginRequest loginRequest) =>
        new(loginRequest.Email,
            loginRequest.Password);
}
