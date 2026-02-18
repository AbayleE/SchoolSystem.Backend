using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class ParentService(BaseRepository<Parent> repo) : BaseService<Parent>(repo);