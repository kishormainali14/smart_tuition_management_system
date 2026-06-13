using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTuitionManagementSystem.Entities;

[Table("FeeCollections")]
public class FeeCollectionEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public int FeeStructureId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Total Amount")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Paid Amount")]
    public decimal PaidAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Remaining Balance")]
    public decimal RemainingBalance { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Due Date")]
    public DateTime DueDate { get; set; }

    [Required]
    [MaxLength(20)]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Pending";

    [MaxLength(500)]
    [Display(Name = "Remarks")]
    public string? Remarks { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("StudentId")]
    public virtual StudentEntity? Student { get; set; }

    [ForeignKey("FeeStructureId")]
    public virtual FeeStructureEntity? FeeStructure { get; set; }

    public virtual ICollection<PaymentTransactionEntity> PaymentTransactions { get; set; } = new List<PaymentTransactionEntity>();
}
