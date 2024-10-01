using RefreshTokenAuthentication.API.Entities;

namespace RefreshTokenAuthentication.API.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    Task<string?> GetNameIdentifierFromTokenAsync(string token);
}
