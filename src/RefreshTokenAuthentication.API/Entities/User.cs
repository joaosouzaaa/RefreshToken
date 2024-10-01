using Microsoft.AspNetCore.Identity;

namespace RefreshTokenAuthentication.API.Entities;

public sealed class User : IdentityUser
{
    public RefreshToken RefreshToken { get; set; } = null!;
}
