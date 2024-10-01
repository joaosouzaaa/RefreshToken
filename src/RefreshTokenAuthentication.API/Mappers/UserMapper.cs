using RefreshTokenAuthentication.API.Arguments;
using RefreshTokenAuthentication.API.DataTransferObjects.User;
using RefreshTokenAuthentication.API.Entities;
using RefreshTokenAuthentication.API.Interfaces.Mappers;

namespace RefreshTokenAuthentication.API.Mappers;

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
