using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class GradesController(BaseService<Grade> service)
    : BaseController<Grade>(service);