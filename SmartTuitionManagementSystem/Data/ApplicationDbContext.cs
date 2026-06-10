using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Entities;
using SmartTuitionManagementSystem.Models;
using TeacherAttendanceSystem.Entities;
using TeacherEntity = SmartTuitionManagementSystem.Entities.TeacherEntity;

namespace SmartTuitionManagementSystem.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ClassEntity> Classes { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<StudentEntity> Students { get; set; }
    public DbSet<TeacherEntity> Teachers { get; set; }
    public DbSet<StudentAttendanceEntity> Attendances { get; set; }
    public DbSet<ExamEntity> Exams { get; set; }
    public DbSet<TeacherAttendance> TeacherAttendances { get; set; } 
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure UserEntity
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("Users");
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
        
        // Configure StudentAttendanceEntity
        modelBuilder.Entity<StudentAttendanceEntity>(entity =>
        {
            entity.ToTable("Attendance");
            entity.HasKey(e => e.Id);
            
            entity.HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // ========== TEACHER ATTENDANCE CONFIGURATION ==========
        
        // Configure TeacherAttendance
        modelBuilder.Entity<TeacherAttendance>(entity =>
        {
            entity.ToTable("TeacherAttendances");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.TeacherId).IsRequired();
            entity.Property(e => e.Date).IsRequired().HasColumnType("date");
            entity.Property(e => e.Status).IsRequired().HasConversion<int>();
            
            // FIXED: datetime → timestamp for PostgreSQL
            entity.Property(e => e.CheckInTime).HasColumnType("timestamp");
            entity.Property(e => e.CheckOutTime).HasColumnType("timestamp");
            
            entity.Property(e => e.TotalWorkingHours).HasColumnType("decimal(5,2)");
            entity.Property(e => e.OvertimeHours).HasColumnType("decimal(5,2)");
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.IsApproved).HasDefaultValue(false);
            entity.Property(e => e.ApprovedDate).HasColumnType("timestamp");
            entity.Property(e => e.RejectionReason).HasMaxLength(200);
            entity.Property(e => e.DocumentPath).HasMaxLength(500);
            
            // FIXED: datetime → timestamp for audit fields
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnType("timestamp");
            
            // Unique constraint
            entity.HasIndex(e => new { e.TeacherId, e.Date })
                .IsUnique()
                .HasDatabaseName("IX_TeacherAttendance_Unique");
            
            // Indexes
            entity.HasIndex(e => e.TeacherId).HasDatabaseName("IX_TeacherAttendance_TeacherId");
            entity.HasIndex(e => e.Date).HasDatabaseName("IX_TeacherAttendance_Date");
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_TeacherAttendance_Status");
            
            // Relationship
            entity.HasOne(e => e.Teacher)
                .WithMany(t => t.Attendances)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}