using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class GradeService(BaseRepository<Grade> repo) : BaseService<Grade>(repo);