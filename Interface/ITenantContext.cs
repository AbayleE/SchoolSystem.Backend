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
        if (claim == null)
            throw new Exception("TenantId missing from JWT");

        TenantId = Guid.Parse(claim);
    }

    public Guid TenantId { get; }
}