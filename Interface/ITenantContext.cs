namespace SchoolSystem.Backend.Interface;

public interface ITenantContext
{
    Guid TenantId { get; }
}

public class TenantContext : ITenantContext
{
    public TenantContext(IHttpContextAccessor accessor, ILogger<TenantContext> logger)
    {
        var http = accessor.HttpContext;
            var claim = http?.User?.FindFirst("tenantId")?.Value ??      
                        http?.User?.FindFirst("TenantId")?.Value ??     
                        http?.User?.FindFirst("tenant_id")?.Value ??    
                        http?.User?.FindFirst("tid")?.Value;
        if (claim == null)
        {
           logger.LogDebug("Claim not found");
        }
        TenantId = Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }

    public Guid TenantId { get; }
}