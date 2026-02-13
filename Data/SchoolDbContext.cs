using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Data;

public class SchoolDbContext(DbContextOptions<SchoolDbContext> options) : DbContext(options)
{
    // DbSets
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Term> Terms { get; set; }
    public DbSet<AcademicYear> AcademicYears { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Parent> Parents { get; set; }
    public DbSet<SchoolSettings> SchoolSettings { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<SystemSettings> SystemSettings { get; set; }
    public DbSet<FileResource> FileResources { get; set; }
    public DbSet<TranscriptRequest> TranscriptRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Application value objects
        modelBuilder.Entity<Application>().OwnsOne(a => a.StudentName);
        modelBuilder.Entity<Application>().OwnsOne(a => a.ParentName);
        modelBuilder.Entity<Application>().OwnsOne(a => a.Parent2Name);
        modelBuilder.Entity<Application>().OwnsOne(a => a.Address);

        // User value object
        modelBuilder.Entity<User>().OwnsOne(u => u.Name);
        modelBuilder.Entity<User>().OwnsOne(u => u.Address);

        base.OnModelCreating(modelBuilder);
    }
}