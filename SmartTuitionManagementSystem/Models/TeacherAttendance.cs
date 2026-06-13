// Models/TeacherAttendance.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartTuitionManagementSystem.Entities;

namespace SmartTuitionManagementSystem.Models;

/// <summary>
/// Attendance Status - Easily extendable for future needs
/// </summary>
public enum AttendanceStatus
{
    [Display(Name = "Present")]
    Present = 1,

    [Display(Name = "Absent")]
    Absent = 2,

    [Display(Name = "Late")]
    Late = 3,

    [Display(Name = "Half Day")]
    HalfDay = 4,

    [Display(Name = "Leave")]
    Leave = 5,

    // Can add more later without breaking existing code:
    // Holiday = 6,
    // Weekend = 7,
    // Remote = 8,
    // Training = 9,
    // SickLeave = 10,
    // CasualLeave = 11
}

/// <summary>
/// Teacher Attendance - Extensible design for future features
/// </summary>
[Table("TeacherAttendances")]
public class TeacherAttendance
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [ForeignKey("Teacher")]
    [Display(Name = "Teacher")]
    public int TeacherId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Column(TypeName = "date")]
    [Display(Name = "Attendance Date")]
    public DateTime Date { get; set; }

    [Required]
    [Display(Name = "Attendance Status")]
    public AttendanceStatus Status { get; set; }

    // Time tracking
    [DataType(DataType.Time)]
    [Column(TypeName = "time")]
    [Display(Name = "Check In Time")]
    public DateTime? CheckInTime { get; set; }

    [DataType(DataType.Time)]
    [Column(TypeName = "time")]
    [Display(Name = "Check Out Time")]
    public DateTime? CheckOutTime { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    [Display(Name = "Total Working Hours")]
    public decimal? TotalWorkingHours { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    [Display(Name = "Overtime Hours")]
    public decimal? OvertimeHours { get; set; }  // Future use

    [MaxLength(500)]
    [Column(TypeName = "varchar(500)")]
    [Display(Name = "Remarks")]
    public string? Remarks { get; set; }

    // Approval workflow (for future)
    [Display(Name = "Is Approved")]
    public bool IsApproved { get; set; } = false;

    [Display(Name = "Approved By")]
    public int? ApprovedBy { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "Approved Date")]
    public DateTime? ApprovedDate { get; set; }

    [MaxLength(200)]
    [Display(Name = "Rejection Reason")]
    public string? RejectionReason { get; set; }  // Future use

    // Document attachment (for future)
    [MaxLength(500)]
    [Display(Name = "Document Path")]
    public string? DocumentPath { get; set; }  // Future use for leave certificates

    // Audit fields
    [Display(Name = "Created At")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Created By")]
    public int? CreatedBy { get; set; }

    [Display(Name = "Updated At")]
    public DateTime? UpdatedAt { get; set; }

    [Display(Name = "Updated By")]
    public int? UpdatedBy { get; set; }

    // Concurrency
    [Timestamp]
    public byte[]? RowVersion { get; set; }

    // Navigation property
    public virtual TeacherEntity? Teacher { get; set; }
}