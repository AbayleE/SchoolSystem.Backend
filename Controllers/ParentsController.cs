using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParentsController(TenantService<Parent> service)
    : BaseController<Parent>(service)
{
}
