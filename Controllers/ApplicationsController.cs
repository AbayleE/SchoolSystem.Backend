using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class ApplicationController(BaseService<Application> service) : BaseController<Application>(service);