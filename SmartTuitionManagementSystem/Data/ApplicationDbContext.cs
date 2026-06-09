using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Entities;

namespace SmartTuitionManagementSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<ClassEntity> Classes { get; set; }
    public DbSet<UserEntity> Users { get; set; }  // ✅ CHANGED: "User" → "Users"
    public DbSet<StudentEntity> Students { get; set; }
    public DbSet<TeacherEntity> Teachers { get; set; }
    public DbSet<AttendanceEntity> Attendances { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure UserEntity
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("Users");  // ✅ ADD THIS to explicitly set table name
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Configure StudentEntity
        modelBuilder.Entity<StudentEntity>(entity =>
        {
            entity.ToTable("Students");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email);
            entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Configure TeacherEntity
        modelBuilder.Entity<TeacherEntity>(entity =>
        {
            entity.ToTable("Teachers");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.HireDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
        });
        
        // Configure AttendanceEntity
        modelBuilder.Entity<AttendanceEntity>(entity =>
        {
            entity.ToTable("Attendance");  // ✅ Match your [Table("Attendance")] attribute
            entity.HasKey(e => e.Id);
            
            // Add relationship configuration
            entity.HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}