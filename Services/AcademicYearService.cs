using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class AcademicYearService(TenantRepository<AcademicYear> repo) : TenantService<AcademicYear>(repo);