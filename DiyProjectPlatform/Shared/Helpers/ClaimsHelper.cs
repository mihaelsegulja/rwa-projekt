using System.Security.Claims;

namespace Shared.Helpers;

public static class ClaimsHelper
{
    public static string GetClaimValue(ClaimsPrincipal claimsPrincipal, string claimType)
    {
        var claim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == claimType);
        return claim?.Value ?? string.Empty;
    }

    public static int GetClaimValueAsInt(ClaimsPrincipal claimsPrincipal, string claimType)
    {
        var claimValue = GetClaimValue(claimsPrincipal, claimType);
        return int.TryParse(claimValue, out var result) ? result : 0;
    }
}
