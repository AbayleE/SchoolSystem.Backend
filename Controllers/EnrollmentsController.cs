using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class EnrollmentsController(BaseService<Enrollment> service)
    : BaseController<Enrollment>(service);