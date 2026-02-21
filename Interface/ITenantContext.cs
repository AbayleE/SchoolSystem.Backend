namespace SchoolSystem.Backend.Interface;

public interface ITenantContext
{
    Guid TenantId { get; }
}

public class TenantContext : ITenantContext
{
    public TenantContext(IHttpContextAccessor accessor)
    {
        var claim = accessor.HttpContext?.User?.FindFirst("TenantId")?.Value;
        TenantId = string.IsNullOrEmpty(claim) ? Guid.Empty : Guid.Parse(claim);
    }

    public Guid TenantId { get; }
}