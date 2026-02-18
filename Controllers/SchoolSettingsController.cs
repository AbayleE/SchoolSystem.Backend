using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class SchoolSettingsController(BaseService<SchoolSettings> service)
    : BaseController<SchoolSettings>(service);