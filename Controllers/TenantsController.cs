using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class TenantController(BaseService<Tenant> service) : BaseController<Tenant>(service);