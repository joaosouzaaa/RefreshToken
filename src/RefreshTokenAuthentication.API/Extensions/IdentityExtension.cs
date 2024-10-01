using System.Security.Claims;
using System.Security.Principal;

namespace RefreshTokenAuthentication.API.Extensions;

public static class IdentityExtension
{
    public static string? GetUserId(this IIdentity identity)
    {
        if (IsIdentityInvalid(identity))
        {
            return null;
        }

        var claimsIdentity = identity as ClaimsIdentity;

        return claimsIdentity!.FindFirst(a => a.Type == ClaimTypes.NameIdentifier)!.Value;
    }

    private static bool IsIdentityInvalid(IIdentity identity) =>
        identity is null || !identity.IsAuthenticated;
}
