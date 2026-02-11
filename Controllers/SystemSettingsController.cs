using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class SystemSettingController(BaseService<SystemSetting> service)
    : BaseController<SystemSetting>(service);