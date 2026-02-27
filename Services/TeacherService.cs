using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;
public class TeacherService(TenantRepository<Teacher> repo) : TenantService<Teacher>(repo);