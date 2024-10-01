using Microsoft.Extensions.Options;
using RefreshTokenAuthentication.API.DataTransferObjects.RefreshToken;
using RefreshTokenAuthentication.API.DataTransferObjects.User;
using RefreshTokenAuthentication.API.Entities;
using RefreshTokenAuthentication.API.Interfaces.Repositories;
using RefreshTokenAuthentication.API.Interfaces.Services;
using RefreshTokenAuthentication.API.Interfaces.Settings;
using RefreshTokenAuthentication.API.Options;
using System.Security.Cryptography;

namespace RefreshTokenAuthentication.API.Services;

public sealed class RefreshTokenService(
    IOptions<TokenOptions> tokenOptions,
    IRefreshTokenRepository refreshTokenRepository,
    IJwtService jwtService,
    IUserRepository userRepository,
    INotificationHandler notificationHandler)
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
            await refreshTokenRepository.AddAsync(new RefreshToken()
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

    public async Task<BearerResponse?> GenerateRefreshTokenAsync(
        GenerateRefreshTokenRequest generateRefreshToken,
        CancellationToken cancellationToken)
    {
        var accessToken = generateRefreshToken.AccessToken;

        var nameIdentifier = await jwtService.GetNameIdentifierFromTokenAsync(accessToken);

        if (nameIdentifier is null)
        {
            notificationHandler.AddNotification("Invalid Claims", "Token does not have the name identifier claim.");

            return null;
        }

        var user = await userRepository.GetByPredicateAsync(u => u.Id == nameIdentifier, cancellationToken);

        if (user is null)
        {
            notificationHandler.AddNotification("Not Found", "User not found.");

            return null;
        }

        var authenticationToken = await userRepository.GetAuthenticationTokenAsync(user);

        if (accessToken != authenticationToken)
        {
            notificationHandler.AddNotification("Invalid Token", "Access Token does not match current token.");

            return null;
        }

        var refreshToken = await refreshTokenRepository.GetByPredicateAsync(r => r.Value == generateRefreshToken.RefreshToken, cancellationToken);

        if (refreshToken is null || refreshToken.ExpiryDate < DateTime.UtcNow)
        {
            notificationHandler.AddNotification("Invalid Refresh Token", "The refresh token informed is invalid.");

            return null;
        }

        var newAuthenticationToken = jwtService.GenerateToken(user);

        var newRefreshToken = await AddOrReplaceRefreshTokenAsync(user.Id, cancellationToken);

        await userRepository.SetAuthenticationTokenAsync(user, newAuthenticationToken);

        return new(
            newAuthenticationToken,
            newRefreshToken.Value,
            newRefreshToken.ExpiryDate);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];

        using var randomNumberGenerator = RandomNumberGenerator.Create();

        randomNumberGenerator.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }
}
