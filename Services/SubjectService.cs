using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class SubjectService(TenantRepository<Subject> repo) : TenantService<Subject>(repo);