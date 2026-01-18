using System.Security.Claims;

namespace BeniaLite.Api.Auth;

public static class CurrentUser
{
    public static Guid GetUserId(ClaimsPrincipal user)
    {
        var uid = user.FindFirstValue("uid");
        return Guid.Parse(uid!);
    }
}
