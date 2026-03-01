using System.Security.Claims;

namespace SchoolSystem.Backend.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("UserId")?.Value;
        return string.IsNullOrEmpty(claim) ? Guid.Empty : Guid.Parse(claim);
    }

    public static Guid GetTenantId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("TenantId")?.Value;
        return string.IsNullOrEmpty(claim) ? Guid.Empty : Guid.Parse(claim);
    }
}

public static class HttpContextExtensions
{
    public static Guid GetTenantIdFromHeader(this HttpContext context)
    {
        // Try to get from Authorization token first
        var tenantId = context.User?.GetTenantId() ?? Guid.Empty;
        if (tenantId != Guid.Empty)
            return tenantId;

        // Try to get from header
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader))
        {
            if (Guid.TryParse(tenantIdHeader.ToString(), out var parsedId))
                return parsedId;
        }

        return Guid.Empty;
    }
    
    public static string? GetUserRole(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Role);
    }
}
