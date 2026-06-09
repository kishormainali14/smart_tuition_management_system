using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTuitionManagementSystem.Entities;

[Table("Classes")]
public class ClassEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Class name is required")]
    [MaxLength(50)]
    [Display(Name = "Class Name")]
    public string ClassName { get; set; } = string.Empty;

    [MaxLength(20)]
    [Display(Name = "Class Code")]
    public string ClassCode { get; set; } = string.Empty;

    [MaxLength(10)]
    [Display(Name = "Section")]
    public string Section { get; set; } = string.Empty;

    [NotMapped]
    public string FullClassName => $"{ClassName} {(string.IsNullOrEmpty(Section) ? "" : "- " + Section)}".Trim();

    [Range(1, 100)]
    [Display(Name = "Maximum Capacity")]
    public int MaxCapacity { get; set; } = 40;

    [Display(Name = "Current Strength")]
    public int CurrentStrength { get; set; } = 0;

    [MaxLength(20)]
    [Display(Name = "Room Number")]
    public string RoomNumber { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Class Teacher ID")]
    public int? ClassTeacherId { get; set; }

    [MaxLength(20)]
    [Display(Name = "Academic Year")]
    public string AcademicYear { get; set; } = DateTime.Now.Year.ToString();

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("ClassTeacherId")]
    public virtual TeacherEntity? ClassTeacher { get; set; }
    
    public virtual ICollection<StudentEntity> Students { get; set; } = new List<StudentEntity>();
    //public virtual ICollection<SubjectAssignmentEntity> SubjectAssignments { get; set; } = new List<SubjectAssignmentEntity>();
}