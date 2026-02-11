using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class TeacherService(BaseRepository<Teacher> repo) : BaseService<Teacher>(repo);