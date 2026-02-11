using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class NotificationService(BaseRepository<Notification> repo) : BaseService<Notification>(repo);