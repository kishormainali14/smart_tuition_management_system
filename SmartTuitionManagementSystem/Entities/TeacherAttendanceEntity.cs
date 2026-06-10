using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeacherAttendanceSystem.Entities
{
    /// <summary>
    /// Represents the attendance status of a teacher
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
        
        [Display(Name = "Holiday")]
        Holiday = 6,
        
        [Display(Name = "Weekend")]
        Weekend = 7
    }
    
    /// <summary>
    /// Teacher Attendance Entity - Maps to database table
    /// </summary>
    [Table("TeacherAttendances")]
    public class TeacherAttendanceEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Teacher ID is required")]
        [Display(Name = "Teacher ID")]
        public int TeacherId { get; set; }
        
        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Attendance Date")]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        
        [Required(ErrorMessage = "Attendance status is required")]
        [Display(Name = "Attendance Status")]
        public AttendanceStatus Status { get; set; }
        
        [Display(Name = "Check In Time")]
        [DataType(DataType.Time)]
        [Column(TypeName = "time")]
        public DateTime? CheckInTime { get; set; }
        
        [Display(Name = "Check Out Time")]
        [DataType(DataType.Time)]
        [Column(TypeName = "time")]
        public DateTime? CheckOutTime { get; set; }
        
        [Display(Name = "Total Working Hours")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? TotalWorkingHours { get; set; }
        
        [MaxLength(500, ErrorMessage = "Remarks cannot exceed 500 characters")]
        [Display(Name = "Remarks")]
        [Column(TypeName = "varchar(500)")]
        public string Remarks { get; set; }
        
        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; } = false;
        
        [Display(Name = "Approved By")]
        public int? ApprovedBy { get; set; }
        
        [Display(Name = "Approved Date")]
        public DateTime? ApprovedDate { get; set; }
        
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }
        
        [Display(Name = "Updated At")]
        public DateTime? UpdatedAt { get; set; }
        
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        
        [Timestamp]
        public byte[] RowVersion { get; set; }
        
        // Navigation properties
        [ForeignKey("TeacherId")]
        public virtual TeacherEntity Teacher { get; set; }
    }
    
    /// <summary>
    /// Teacher Entity - Parent table
    /// </summary>
    [Table("Teachers")]
    public class TeacherEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        [Required]
        [MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }
        
        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseSalary { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PerDayRate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public virtual ICollection<TeacherAttendanceEntity> Attendances { get; set; }
        
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
    
    /// <summary>
    /// DTO for Attendance with Teacher Details
    /// </summary>
    public class TeacherAttendanceDto
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public DateTime Date { get; set; }
        public AttendanceStatus Status { get; set; }
        public string StatusText { get; set; }
        public string StatusColor { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
        public decimal? TotalWorkingHours { get; set; }
        public string Remarks { get; set; }
        public bool IsApproved { get; set; }
        
        public static TeacherAttendanceDto FromEntity(TeacherAttendanceEntity entity)
        {
            return new TeacherAttendanceDto
            {
                Id = entity.Id,
                TeacherId = entity.TeacherId,
                TeacherName = entity.Teacher?.FullName ?? string.Empty,
                Date = entity.Date,
                Status = entity.Status,
                StatusText = GetStatusText(entity.Status),
                StatusColor = GetStatusColor(entity.Status),
                CheckInTime = entity.CheckInTime?.ToString("HH:mm"),
                CheckOutTime = entity.CheckOutTime?.ToString("HH:mm"),
                TotalWorkingHours = entity.TotalWorkingHours,
                Remarks = entity.Remarks,
                IsApproved = entity.IsApproved
            };
        }
        
        private static string GetStatusText(AttendanceStatus status)
        {
            return status switch
            {
                AttendanceStatus.Present => "Present",
                AttendanceStatus.Absent => "Absent",
                AttendanceStatus.Late => "Late",
                AttendanceStatus.HalfDay => "Half Day",
                AttendanceStatus.Leave => "Leave",
                AttendanceStatus.Holiday => "Holiday",
                AttendanceStatus.Weekend => "Weekend",
                _ => "Unknown"
            };
        }
        
        private static string GetStatusColor(AttendanceStatus status)
        {
            return status switch
            {
                AttendanceStatus.Present => "success",
                AttendanceStatus.Absent => "danger",
                AttendanceStatus.Late => "warning",
                AttendanceStatus.HalfDay => "info",
                AttendanceStatus.Leave => "secondary",
                AttendanceStatus.Holiday => "primary",
                AttendanceStatus.Weekend => "dark",
                _ => "light"
            };
        }
    }
    
    /// <summary>
    /// ViewModel for marking attendance
    /// </summary>
    public class MarkAttendanceViewModel
    {
        [DataType(DataType.Date)]
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        
        public List<TeacherAttendanceEntry> Teachers { get; set; }
    }
    
    public class TeacherAttendanceEntry
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public AttendanceStatus? Status { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
        public string Remarks { get; set; }
        public int? AttendanceId { get; set; }
    }
    
    /// <summary>
    /// Monthly attendance summary
    /// </summary>
    public class MonthlyAttendanceSummary
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LateDays { get; set; }
        public int HalfDays { get; set; }
        public int LeaveDays { get; set; }
        public int HolidayDays { get; set; }
        public int WeekendDays { get; set; }
        public decimal AttendancePercentage { get; set; }
        public decimal CalculatedSalary { get; set; }
    }
    
    /// <summary>
    /// Attendance report filter
    /// </summary>
    public class AttendanceReportFilter
    {
        public int? TeacherId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public AttendanceStatus? Status { get; set; }
        public bool? IsApproved { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    
    /// <summary>
    /// Paged result for attendance
    /// </summary>
    public class PagedAttendanceResult
    {
        public List<TeacherAttendanceDto> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}