using RefreshTokenAuthentication.API.Arguments;
using RefreshTokenAuthentication.API.DataTransferObjects.User;
using RefreshTokenAuthentication.API.Entities;

namespace RefreshTokenAuthentication.API.Interfaces.Mappers;

public interface IUserMapper
{
    User CreateRequestToDomain(CreateUserRequest createRequest);
    GetUserByIdResponse DomainToGetByIdResponse(User user);
    LoginArgument LoginRequestToDomain(LoginRequest loginRequest);
}
