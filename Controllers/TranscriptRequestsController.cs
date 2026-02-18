using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

public class TranscriptRequestController(BaseService<TranscriptRequest> service)
    : BaseController<TranscriptRequest>(service);