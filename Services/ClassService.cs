using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class ClassService(BaseRepository<Class> repo) : BaseService<Class>(repo);