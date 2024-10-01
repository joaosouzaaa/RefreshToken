using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RefreshTokenAuthentication.API.DataTransferObjects.RefreshToken;
using RefreshTokenAuthentication.API.DataTransferObjects.User;
using RefreshTokenAuthentication.API.Interfaces.Services;
using RefreshTokenAuthentication.API.Settings.NotificationSettings;

namespace RefreshTokenAuthentication.API.Controllers;

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
