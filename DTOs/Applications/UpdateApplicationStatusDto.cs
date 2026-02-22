namespace SchoolSystem.Backend.DTOs.Applications;

public class UpdateApplicationStatusDto
{
    public required string Status { get; set; } // "pending", "accepted", "rejected"
    public string? Notes { get; set; }
    public bool SendNotification { get; set; } = true;
}
