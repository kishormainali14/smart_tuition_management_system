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
    
    // Fee Management
    public DbSet<FeeTypeEntity> FeeTypes { get; set; }
    public DbSet<FeeStructureEntity> FeeStructures { get; set; }
    public DbSet<FeeCollectionEntity> FeeCollections { get; set; }
    public DbSet<PaymentTransactionEntity> PaymentTransactions { get; set; }
    
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
            
            entity.Property(e => e.CheckInTime).HasColumnType("timestamp");
            entity.Property(e => e.CheckOutTime).HasColumnType("timestamp");
            
            entity.Property(e => e.TotalWorkingHours).HasColumnType("decimal(5,2)");
            entity.Property(e => e.OvertimeHours).HasColumnType("decimal(5,2)");
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.IsApproved).HasDefaultValue(false);
            entity.Property(e => e.ApprovedDate).HasColumnType("timestamp");
            entity.Property(e => e.RejectionReason).HasMaxLength(200);
            entity.Property(e => e.DocumentPath).HasMaxLength(500);
            
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnType("timestamp");
            
            entity.HasIndex(e => new { e.TeacherId, e.Date })
                .IsUnique()
                .HasDatabaseName("IX_TeacherAttendance_Unique");
            
            entity.HasIndex(e => e.TeacherId).HasDatabaseName("IX_TeacherAttendance_TeacherId");
            entity.HasIndex(e => e.Date).HasDatabaseName("IX_TeacherAttendance_Date");
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_TeacherAttendance_Status");
            
            entity.HasOne(e => e.Teacher)
                .WithMany(t => t.Attendances)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ========== FEE MANAGEMENT CONFIGURATION ==========

        // Configure FeeTypeEntity
        modelBuilder.Entity<FeeTypeEntity>(entity =>
        {
            entity.ToTable("FeeTypes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FeeTypeName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Configure FeeStructureEntity
        modelBuilder.Entity<FeeStructureEntity>(entity =>
        {
            entity.ToTable("FeeStructures");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(f => f.Class)
                .WithMany()
                .HasForeignKey(f => f.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.FeeType)
                .WithMany(ft => ft.FeeStructures)
                .HasForeignKey(f => f.FeeTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(f => new { f.ClassId, f.FeeTypeId }).IsUnique();
        });

        // Configure FeeCollectionEntity
        modelBuilder.Entity<FeeCollectionEntity>(entity =>
        {
            entity.ToTable("FeeCollections");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            entity.Property(e => e.RemainingBalance).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Pending");
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(f => f.Student)
                .WithMany()
                .HasForeignKey(f => f.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.FeeStructure)
                .WithMany(fs => fs.FeeCollections)
                .HasForeignKey(f => f.FeeStructureId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(f => new { f.StudentId, f.FeeStructureId }).IsUnique();
        });

        // Configure PaymentTransactionEntity
        modelBuilder.Entity<PaymentTransactionEntity>(entity =>
        {
            entity.ToTable("PaymentTransactions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TransactionReference).HasMaxLength(100);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(t => t.FeeCollection)
                .WithMany(fc => fc.PaymentTransactions)
                .HasForeignKey(t => t.FeeCollectionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Student)
                .WithMany()
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(t => t.PaymentDate);
        });
    }
}