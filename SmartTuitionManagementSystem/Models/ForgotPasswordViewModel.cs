using System.ComponentModel.DataAnnotations;

namespace SmartTuitionManagementSystem.Models;

public class ForgotPasswordViewModel
{
    // FIX: Class was completely empty — ModelState.IsValid always passed,
    // and model.Email reference would throw NullReferenceException
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;
}