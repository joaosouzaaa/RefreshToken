using RefreshToken.API.Arguments;
using RefreshToken.API.DataTransferObjects.User;
using RefreshToken.API.Entities;

namespace RefreshToken.API.Interfaces.Mappers;

public interface IUserMapper
{
    User CreateRequestToDomain(CreateUserRequest createRequest);
    GetUserByIdResponse DomainToGetByIdResponse(User user);
    LoginArgument LoginRequestToDomain(LoginRequest loginRequest);
}
