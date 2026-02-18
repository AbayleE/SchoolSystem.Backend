using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class SubjectsController(BaseService<Subject> service)
    : BaseController<Subject>(service);