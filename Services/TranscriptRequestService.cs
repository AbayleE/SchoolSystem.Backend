using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class TranscriptRequestService(TenantRepository<TranscriptRequest> repo) : TenantService<TranscriptRequest>(repo);