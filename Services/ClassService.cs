using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class ClassService(TenantRepository<Class> repo) : TenantService<Class>(repo);