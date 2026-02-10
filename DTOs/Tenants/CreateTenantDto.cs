namespace SchoolSystem.Backend.DTOs.Tenants;

public class CreateTenantDto
{
    public required string Name { get; set; }
    public string? Subdomain { get; set; }
    public string? LogoUrl { get; set; }
}