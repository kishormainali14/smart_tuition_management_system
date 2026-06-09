using System.ComponentModel.DataAnnotations;

namespace SmartTuitionManagementSystem.Models;

public class Teacher
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

    [Required(ErrorMessage = "Qualification is required")]
    [StringLength(200)]
    [Display(Name = "Qualification")]
    public string Qualification { get; set; } = string.Empty;

    [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years")]
    [Display(Name = "Years of Experience")]
    public int ExperienceYears { get; set; }

    [StringLength(200)]
    [Display(Name = "Specialization")]
    public string Specialization { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Hire Date")]
    public DateTime HireDate { get; set; } = DateTime.Today;

    [Range(0, 999999.99, ErrorMessage = "Salary must be between 0 and 999,999.99")]
    [DataType(DataType.Currency)]
    [Display(Name = "Salary")]
    public decimal Salary { get; set; }
}