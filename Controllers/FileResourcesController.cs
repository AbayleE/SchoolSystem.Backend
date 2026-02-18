using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class FileResourceController(BaseService<FileResource> service) : BaseController<FileResource>(service);