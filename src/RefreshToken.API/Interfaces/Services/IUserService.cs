using RefreshToken.API.DataTransferObjects.User;

namespace RefreshToken.API.Interfaces.Services;

public interface IUserService
{
    Task CreateAsync(CreateUserRequest createUser, CancellationToken cancellationToken);
    Task<GetUserByIdResponse?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<BearerResponse?> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken);
    Task SignOutAsync(string userId, CancellationToken cancellationToken);
}
