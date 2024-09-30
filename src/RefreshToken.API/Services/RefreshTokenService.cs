using Microsoft.Extensions.Options;
using RefreshToken.API.DataTransferObjects.RefreshToken;
using RefreshToken.API.Entities;
using RefreshToken.API.Interfaces.Repositories;
using RefreshToken.API.Interfaces.Services;
using RefreshToken.API.Options;
using System.Security.Cryptography;

namespace RefreshToken.API.Services;

public sealed class RefreshTokenService(
    IOptions<TokenOptions> tokenOptions,
    IRefreshTokenRepository refreshTokenRepository)
    : IRefreshTokenService
{
    private readonly TokenOptions _token = tokenOptions.Value;

    public async Task<AddOrUpdateRefreshTokenResponse> AddOrReplaceRefreshTokenAsync(string userId, CancellationToken cancellationToken)
    {
        var refreshToken = await refreshTokenRepository.GetByPredicateAsync(r => r.UserId == userId, cancellationToken);

        var expiryDate = DateTime.UtcNow.AddMinutes(_token.RefreshTokenExpirationTimeInMinutes);
        var refreshTokenValue = GenerateRefreshToken();

        if (refreshToken is null)
        {
            await refreshTokenRepository.AddAsync(new ApplicationRefreshToken()
            {
                ExpiryDate = expiryDate,
                UserId = userId,
                Value = refreshTokenValue
            },
            cancellationToken);
        }
        else
        {
            refreshToken.Value = refreshTokenValue;
            refreshToken.ExpiryDate = expiryDate;

            await refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);
        }

        return new(
            refreshTokenValue,
            expiryDate);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];

        using var randomNumberGenerator = RandomNumberGenerator.Create();

        randomNumberGenerator.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }
}
