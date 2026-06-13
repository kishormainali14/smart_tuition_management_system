using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartTuitionManagementSystem.Models;

namespace SmartTuitionManagementSystem.Entities;

[Table("Teachers")]
public class TeacherEntity
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

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999.99)]
    [Display(Name = "Salary")]
    public decimal Salary { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }

    // Navigation property for attendance records
    public virtual ICollection<TeacherAttendance>? Attendances { get; set; }
}