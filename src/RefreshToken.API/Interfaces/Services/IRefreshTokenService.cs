using RefreshToken.API.DataTransferObjects.RefreshToken;
using RefreshToken.API.DataTransferObjects.User;

namespace RefreshToken.API.Interfaces.Services;

public interface IRefreshTokenService
{
    Task<AddOrUpdateRefreshTokenResponse> AddOrReplaceRefreshTokenAsync(string userId, CancellationToken cancellationToken);
    Task<BearerResponse?> GenerateRefreshTokenAsync(
        GenerateRefreshTokenRequest generateRefreshToken,
        CancellationToken cancellationToken);
}
