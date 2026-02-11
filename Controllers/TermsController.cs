using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class TermsController(BaseService<Term> service)
    : BaseController<Term>(service);