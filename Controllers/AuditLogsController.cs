using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class AuditLogsController(BaseService<AuditLog> service) : BaseController<AuditLog>(service);