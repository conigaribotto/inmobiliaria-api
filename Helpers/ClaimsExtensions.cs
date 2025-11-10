using System.Security.Claims;

namespace InmobiliariaAPI.Helpers;

public static class ClaimsExtensions
{
    public static int PropietarioId(this ClaimsPrincipal user)
    {
        var str = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(str)) throw new InvalidOperationException("Token sin NameIdentifier");
        return int.Parse(str);
    }
}
