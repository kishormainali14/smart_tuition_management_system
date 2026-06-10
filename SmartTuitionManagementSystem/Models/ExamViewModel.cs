using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SmartTuitionManagementSystem.Models;

public class ExamViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Exam name is required")]
    [StringLength(100, MinimumLength = 3)]
    [Display(Name = "Exam Name")]
    public string ExamName { get; set; } = string.Empty;

    [Display(Name = "Exam Code")]
    public string ExamCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Exam type is required")]
    [Display(Name = "Exam Type")]
    public string ExamType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Academic year is required")]
    [Display(Name = "Academic Year")]
    public string AcademicYear { get; set; } = string.Empty;

    [Required(ErrorMessage = "Section required")]
    [Display(Name = "Section")]
    public string Section { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Status")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // For dropdown lists
    public IEnumerable<SelectListItem> ExamTypeOptions { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> AcademicYearOptions { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> SectionOptions { get; set; } = new List<SelectListItem>();
}