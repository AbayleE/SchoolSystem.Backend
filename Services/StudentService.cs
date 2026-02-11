using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class StudentService(BaseRepository<Student> repo) : BaseService<Student>(repo);