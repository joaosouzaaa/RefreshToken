using RefreshToken.API.Entities;

namespace RefreshToken.API.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    Task<string?> GetNameIdentifierFromTokenAsync(string token);
}
