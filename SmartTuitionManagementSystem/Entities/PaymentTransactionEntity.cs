using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTuitionManagementSystem.Entities;

[Table("PaymentTransactions")]
public class PaymentTransactionEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int FeeCollectionId { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Amount Paid")]
    public decimal AmountPaid { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Payment Date")]
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(50)]
    [Display(Name = "Payment Method")]
    public string PaymentMethod { get; set; } = "Cash";

    [MaxLength(100)]
    [Display(Name = "Transaction Reference")]
    public string? TransactionReference { get; set; }

    [MaxLength(500)]
    [Display(Name = "Remarks")]
    public string? Remarks { get; set; }

    [Display(Name = "Is Cancelled")]
    public bool IsCancelled { get; set; } = false;

    public DateTime? CancelledAt { get; set; }

    public int? CancelledBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("FeeCollectionId")]
    public virtual FeeCollectionEntity? FeeCollection { get; set; }

    [ForeignKey("StudentId")]
    public virtual StudentEntity? Student { get; set; }
}
