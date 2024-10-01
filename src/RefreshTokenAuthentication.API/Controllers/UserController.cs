using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RefreshTokenAuthentication.API.DataTransferObjects.User;
using RefreshTokenAuthentication.API.Extensions;
using RefreshTokenAuthentication.API.Interfaces.Services;
using RefreshTokenAuthentication.API.Settings.NotificationSettings;

namespace RefreshTokenAuthentication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class UserController(IUserService userService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<Notification>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task CreateAsync([FromBody] CreateUserRequest createUserRequest, CancellationToken cancellationToken) =>
        userService.CreateAsync(createUserRequest, cancellationToken);

    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserByIdResponse))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<GetUserByIdResponse?> GetByIdAsync(CancellationToken cancellationToken) =>
        userService.GetByIdAsync(User.Identity!.GetUserId()!, cancellationToken);

    [AllowAnonymous]
    [HttpPost("authenticate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BearerResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<Notification>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<BearerResponse?> LoginAsync([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken) =>
        userService.LoginAsync(loginRequest, cancellationToken);

    [Authorize]
    [HttpPost("sign-out")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task SignOutAsync(CancellationToken cancellationToken) =>
        userService.SignOutAsync(User.Identity!.GetUserId()!, cancellationToken);
}
