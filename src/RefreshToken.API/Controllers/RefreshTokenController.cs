using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RefreshToken.API.DataTransferObjects.RefreshToken;
using RefreshToken.API.DataTransferObjects.User;
using RefreshToken.API.Interfaces.Services;
using RefreshToken.API.Settings.NotificationSettings;

namespace RefreshToken.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class RefreshTokenController(IRefreshTokenService refreshTokenService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("generate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BearerResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<Notification>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<BearerResponse?> GenerateRefreshTokenAsync(
        [FromBody] GenerateRefreshTokenRequest generateRefreshToken,
        CancellationToken cancellationToken) =>
        refreshTokenService.GenerateRefreshTokenAsync(generateRefreshToken, cancellationToken);
}
