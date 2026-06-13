using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTuitionManagementSystem.Entities;

[Table("FeeStructures")]
public class FeeStructureEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int ClassId { get; set; }

    [Required]
    public int FeeTypeId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Due Date")]
    public DateTime? DueDate { get; set; }

    [MaxLength(500)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ClassId")]
    public virtual ClassEntity? Class { get; set; }

    [ForeignKey("FeeTypeId")]
    public virtual FeeTypeEntity? FeeType { get; set; }

    public virtual ICollection<FeeCollectionEntity> FeeCollections { get; set; } = new List<FeeCollectionEntity>();
}
