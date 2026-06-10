// Models/Teacher.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTuitionManagementSystem.Models;

public class Teacher
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // From TeacherAttendanceSystem.Entities.TeacherEntity
    [Required]
    [MaxLength(50)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    // From SmartTuitionManagementSystem.Entities.TeacherEntity
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "Address")]
    public string Address { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Display(Name = "Qualification")]
    public string Qualification { get; set; } = string.Empty;

    [Range(0, 50)]
    [Display(Name = "Years of Experience")]
    public int ExperienceYears { get; set; }

    [MaxLength(200)]
    [Display(Name = "Specialization")]
    public string Specialization { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Hire Date")]
    public DateTime HireDate { get; set; } = DateTime.UtcNow;

    // Salary from both - using decimal(18,2)
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999.99)]
    [Display(Name = "Monthly Salary")]
    public decimal Salary { get; set; }

    // From TeacherAttendanceSystem (for per-day calculation)
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Per Day Rate")]
    public decimal PerDayRate { get; set; }

    // From both
    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }

    // Computed property
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    // Navigation property
    public virtual ICollection<TeacherAttendance> Attendances { get; set; } = new List<TeacherAttendance>();
}