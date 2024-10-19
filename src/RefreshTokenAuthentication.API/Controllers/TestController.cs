using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RefreshTokenAuthentication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class TestController : ControllerBase
{
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult TestAuthentication() =>
        Ok();
}
