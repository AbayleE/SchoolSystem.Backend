using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class NotificationsController(BaseService<Notification> service)
    : BaseController<Notification>(service);