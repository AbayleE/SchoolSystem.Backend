using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class StudentService(TenantRepository<Student> repo) : TenantService<Student>(repo);