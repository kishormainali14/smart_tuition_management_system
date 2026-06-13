using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartTuitionManagementSystem.Models;

public class FeeStructureViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Class is required")]
    [Display(Name = "Class")]
    public int ClassId { get; set; }

    [Required(ErrorMessage = "Fee type is required")]
    [Display(Name = "Fee Type")]
    public int FeeTypeId { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Amount must be between 0.01 and 999,999.99")]
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Due Date")]
    public DateTime? DueDate { get; set; }

    [MaxLength(500)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public string? ClassName { get; set; }
    public string? FeeTypeName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public IEnumerable<SelectListItem> ClassList { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> FeeTypeList { get; set; } = new List<SelectListItem>();
}

public class FeeCollectionViewModel
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int FeeStructureId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingBalance { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Remarks { get; set; }

    public string? StudentName { get; set; }
    public string? RollNumber { get; set; }
    public string? ClassName { get; set; }
    public string? FeeTypeName { get; set; }
    public string? ParentPhone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public IEnumerable<SelectListItem> ClassList { get; set; } = new List<SelectListItem>();
}

public class FeePaymentViewModel
{
    public int FeeCollectionId { get; set; }
    public int StudentId { get; set; }
    public decimal RemainingBalance { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Amount must be between 0.01 and 999,999.99")]
    [Display(Name = "Amount to Pay")]
    public decimal AmountPaid { get; set; }

    [Required(ErrorMessage = "Payment date is required")]
    [DataType(DataType.Date)]
    [Display(Name = "Payment Date")]
    public DateTime PaymentDate { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Payment method is required")]
    [Display(Name = "Payment Method")]
    public string PaymentMethod { get; set; } = "Cash";

    [MaxLength(100)]
    [Display(Name = "Transaction Reference")]
    public string? TransactionReference { get; set; }

    [MaxLength(500)]
    [Display(Name = "Remarks")]
    public string? Remarks { get; set; }

    public IEnumerable<SelectListItem> PaymentMethodList { get; set; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "Cash", Text = "Cash" },
        new SelectListItem { Value = "Bank Transfer", Text = "Bank Transfer" },
        new SelectListItem { Value = "Online", Text = "Online Payment" },
        new SelectListItem { Value = "Cheque", Text = "Cheque" }
    };
}

public class PaymentTransactionViewModel
{
    public int Id { get; set; }
    public int FeeCollectionId { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? RollNumber { get; set; }
    public string? ClassName { get; set; }
    public string? FeeTypeName { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionReference { get; set; }
    public string? Remarks { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class FeeDashboardViewModel
{
    public decimal TotalCollected { get; set; }
    public decimal TotalDue { get; set; }
    public decimal TodayCollection { get; set; }
    public decimal MonthlyCollection { get; set; }
    public int TotalStudentsWithDue { get; set; }
    public int OverdueCount { get; set; }
    public List<FeeCollectionViewModel> RecentCollections { get; set; } = new();
    public List<PaymentTransactionViewModel> RecentPayments { get; set; } = new();
}

public class FeeReportViewModel
{
    [Display(Name = "Report Type")]
    public string ReportType { get; set; } = "Class";

    [Display(Name = "Class")]
    public int? ClassId { get; set; }

    [Display(Name = "Student")]
    public int? StudentId { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "From Date")]
    public DateTime? FromDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "To Date")]
    public DateTime? ToDate { get; set; }

    public IEnumerable<SelectListItem> ClassList { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> StudentList { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> ReportTypeList { get; set; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "Daily", Text = "Daily Collection" },
        new SelectListItem { Value = "Monthly", Text = "Monthly Collection" },
        new SelectListItem { Value = "Class", Text = "Class-wise Collection" },
        new SelectListItem { Value = "Student", Text = "Student-wise Collection" },
        new SelectListItem { Value = "Outstanding", Text = "Outstanding Dues" },
        new SelectListItem { Value = "PaymentHistory", Text = "Payment History" }
    };

    public List<FeeCollectionViewModel> Collections { get; set; } = new();
    public List<PaymentTransactionViewModel> Payments { get; set; } = new();
    public decimal GrandTotalCollected { get; set; }
    public decimal GrandTotalDue { get; set; }
    public decimal GrandTotalRemaining { get; set; }
}

public class FeeReceiptViewModel
{
    public int TransactionId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public string? StudentName { get; set; }
    public string? RollNumber { get; set; }
    public string? ClassName { get; set; }
    public string? ParentName { get; set; }
    public string? ParentPhone { get; set; }
    public string? FeeTypeName { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal RemainingBalance { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionReference { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime GeneratedAt { get; set; }
}
