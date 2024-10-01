using RefreshTokenAuthentication.API.DataTransferObjects.RefreshToken;
using RefreshTokenAuthentication.API.DataTransferObjects.User;

namespace RefreshTokenAuthentication.API.Interfaces.Services;

public interface IRefreshTokenService
{
    Task<AddOrUpdateRefreshTokenResponse> AddOrReplaceRefreshTokenAsync(string userId, CancellationToken cancellationToken);
    Task<BearerResponse?> GenerateRefreshTokenAsync(
        GenerateRefreshTokenRequest generateRefreshToken,
        CancellationToken cancellationToken);
}
