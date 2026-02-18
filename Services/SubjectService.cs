using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class SubjectService(BaseRepository<Subject> repo) : BaseService<Subject>(repo);