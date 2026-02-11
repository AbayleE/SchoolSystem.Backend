using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class ApplicationDocumentController(BaseService<ApplicationDocument> service)
    : BaseController<ApplicationDocument>(service);