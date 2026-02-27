using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class TeacherController(TenantService<Teacher> service) : BaseController<Teacher>(service);