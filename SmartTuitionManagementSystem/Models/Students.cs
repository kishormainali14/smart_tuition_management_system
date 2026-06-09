using System.ComponentModel.DataAnnotations;

namespace SmartTuitionManagementSystem.Models;

public class Students
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Address")]
    public string Address { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(50)]
    [Display(Name = "Grade/Class")]
    public string Grade { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Parent/Guardian Name")]
    public string ParentName { get; set; } = string.Empty;

    [StringLength(20)]
    [Phone(ErrorMessage = "Invalid phone number")]
    [Display(Name = "Parent Phone")]
    public string ParentPhone { get; set; } = string.Empty;
}