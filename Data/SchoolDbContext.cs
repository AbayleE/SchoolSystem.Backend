using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Data;

public class SchoolDbContext : DbContext
{
    public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Invitation> Invitations {get; set;}
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<FileResource> FileResources { get; set; }
    public DbSet<TranscriptRequest> TranscriptRequests { get; set; }
}