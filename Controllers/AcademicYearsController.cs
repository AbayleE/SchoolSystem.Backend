using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class AcademicYearController(TenantService<AcademicYear> service) : BaseController<AcademicYear>(service);