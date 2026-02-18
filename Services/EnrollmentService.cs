using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class EnrollmentService(BaseRepository<Enrollment> repo) : BaseService<Enrollment>(repo);