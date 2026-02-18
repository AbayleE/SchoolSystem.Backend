using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class TeacherController(BaseService<Teacher> service) : BaseController<Teacher>(service);