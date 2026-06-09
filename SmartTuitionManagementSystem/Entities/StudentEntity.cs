using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTuitionManagementSystem.Entities;

[Table("Students")]
public class StudentEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

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

    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime? DateOfBirth { get; set; }

    [MaxLength(50)]
    [Display(Name = "Grade/Class")]
    public string Grade { get; set; } = string.Empty;

    [MaxLength(100)]
    [Display(Name = "Parent/Guardian Name")]
    public string ParentName { get; set; } = string.Empty;

    [MaxLength(20)]
    [Phone]
    [Display(Name = "Parent Phone")]
    public string ParentPhone { get; set; } = string.Empty;

    [Display(Name = "Enrollment Date")]
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    [Display(Name = "Class ID")]
    public int ClassId { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    [ForeignKey("ClassId")]
    public virtual ClassEntity? Class { get; set; }
    
    // FIXED: Added initialization
    public virtual ICollection<AttendanceEntity> Attendances { get; set; } = new List<AttendanceEntity>();
}