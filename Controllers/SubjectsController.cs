using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class SubjectsController(TenantService<Subject> service)
    : BaseController<Subject>(service);