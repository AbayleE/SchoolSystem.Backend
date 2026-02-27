using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class AnnouncementService(TenantRepository<Announcement> repo) : TenantService<Announcement>(repo);