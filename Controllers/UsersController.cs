using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class UserController(BaseService<User> service) : BaseController<User>(service);