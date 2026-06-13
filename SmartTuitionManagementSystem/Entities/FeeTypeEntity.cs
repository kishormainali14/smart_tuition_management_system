using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTuitionManagementSystem.Entities;

[Table("FeeTypes")]
public class FeeTypeEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Display(Name = "Fee Type Name")]
    public string FeeTypeName { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<FeeStructureEntity> FeeStructures { get; set; } = new List<FeeStructureEntity>();
}
