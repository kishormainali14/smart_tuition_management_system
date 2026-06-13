using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartTuitionManagementSystem.Models;

public class StudentViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(20)]
    [Display(Name = "Roll Number")]
    public string RollNumber { get; set; } = string.Empty;

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

    [Required(ErrorMessage = "Class is required")]
    [Display(Name = "Class")]
    public int ClassId { get; set; }

    [StringLength(100)]
    [Display(Name = "Parent/Guardian Name")]
    public string ParentName { get; set; } = string.Empty;

    [StringLength(20)]
    [Phone(ErrorMessage = "Invalid phone number")]
    [Display(Name = "Parent Phone")]
    public string ParentPhone { get; set; } = string.Empty;

    [Display(Name = "Enrollment Date")]
    [DataType(DataType.Date)]
    public DateTime EnrollmentDate { get; set; } = DateTime.Now;

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    // Add this property
    [Display(Name = "Class Name")]
    public string? ClassName { get; set; }

    // Dropdown lists
    public IEnumerable<SelectListItem> ClassList { get; set; } = new List<SelectListItem>();
}