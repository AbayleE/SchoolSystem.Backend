using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class GradeService(TenantRepository<Grade> repo) : TenantService<Grade>(repo);