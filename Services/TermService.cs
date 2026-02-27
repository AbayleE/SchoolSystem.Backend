using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;
public class TermService(TenantRepository<Term> repo) : TenantService<Term>(repo);