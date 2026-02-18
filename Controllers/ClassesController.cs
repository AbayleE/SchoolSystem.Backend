using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class ClassesController(BaseService<Class> service)
    : BaseController<Class>(service);