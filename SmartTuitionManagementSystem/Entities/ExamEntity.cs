using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTuitionManagementSystem.Entities;

[Table("Exams")]
public class ExamEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Exam name is required")]
    [StringLength(100, MinimumLength = 3)]
    [Display(Name = "Exam Name")]
    public string ExamName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Exam code is required")]
    [StringLength(20)]
    [Display(Name = "Exam Code")]
    public string ExamCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Exam type is required")]
    [StringLength(50)]
    [Display(Name = "Exam Type")]
    public string ExamType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Academic year is required")]
    [StringLength(20)]
    [Display(Name = "Academic Year")]
    public string AcademicYear { get; set; } = string.Empty;

    [Required(ErrorMessage = "Section is required")]
    [StringLength(20)]
    [Display(Name = "Section")]
    public string Section { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Status")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Created Date")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Last Modified")]
    public DateTime? UpdatedAt { get; set; }
}