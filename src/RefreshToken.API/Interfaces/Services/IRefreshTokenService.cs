using RefreshToken.API.DataTransferObjects.RefreshToken;

namespace RefreshToken.API.Interfaces.Services;

public interface IRefreshTokenService
{
    Task<AddOrUpdateRefreshTokenResponse> AddOrReplaceRefreshTokenAsync(string userId, CancellationToken cancellationToken);
}
