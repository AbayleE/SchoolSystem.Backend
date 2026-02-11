using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class TenantService(BaseRepository<Tenant> repo)
    : BaseService<Tenant>(repo)
{
}