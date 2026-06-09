using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SmartTuitionManagementSystem.Models;

public class ClassViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Class name is required")]
    [StringLength(50, MinimumLength = 2)]
    [Display(Name = "Class Name")]
    public string ClassName { get; set; } = string.Empty;

    [Display(Name = "Class Code")]
    [StringLength(20)]
    public string ClassCode { get; set; } = string.Empty;

    // ADDED: Grade property
    [Display(Name = "Grade")]
    public string Grade { get; set; } = string.Empty;

    [Display(Name = "Section")]
    [StringLength(10)]
    public string Section { get; set; } = string.Empty;

    // Updated FullClassName to include Grade
    [Display(Name = "Full Class Name")]
    public string FullClassName
    {
        get 
        {
            string gradePart = string.IsNullOrEmpty(Grade) ? "" : Grade;
            string sectionPart = string.IsNullOrEmpty(Section) ? "" : $"- {Section}";
            return $"{gradePart} {ClassName} {sectionPart}".Trim();
        }
        set => throw new NotImplementedException();
    }

    [Required]
    [Range(1, 100)]
    [Display(Name = "Maximum Capacity")]
    public int MaxCapacity { get; set; } = 40;

    [Display(Name = "Current Strength")]
    public int CurrentStrength { get; set; }

    [Display(Name = "Available Seats")]
    public int AvailableSeats => MaxCapacity - CurrentStrength;

    [Display(Name = "Room Number")]
    public string RoomNumber { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Class Teacher")]
    public int? ClassTeacherId { get; set; }

    [Display(Name = "Class Teacher")]
    public string ClassTeacherName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Academic Year")]
    public string AcademicYear { get; set; } = DateTime.Now.Year.ToString();

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    public int TotalStudents { get; set; }
    public int TotalSubjects { get; set; }
    public int TotalTeachers { get; set; }

    // ADDED: Grade options for dropdown
    public IEnumerable<SelectListItem> GradeOptions { get; set; } = new List<SelectListItem>();
    
    public IEnumerable<SelectListItem> TeacherOptions { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> AcademicYearOptions { get; set; } = new List<SelectListItem>();

    public List<SubjectAssignmentViewModel> SubjectAssignments { get; set; } = new List<SubjectAssignmentViewModel>();
    public List<ClassStudentViewModel> Students { get; set; } = new List<ClassStudentViewModel>();
}

// REMOVED: GradeHelper class - moved to controller instead

public class SubjectAssignmentViewModel
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public int? TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string ScheduleDay { get; set; } = string.Empty;
    public string ScheduleTime { get; set; } = string.Empty;
    public DateTime AssignedDate { get; set; }
}

public class ClassStudentViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string RollNumber { get; set; } = string.Empty;
}

public class SubjectViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Subject name is required")]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Subject Name")]
    public string SubjectName { get; set; } = string.Empty;

    [Display(Name = "Subject Code")]
    [StringLength(20)]
    public string SubjectCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Subject Type")]
    public string SubjectType { get; set; } = "Theory";

    [Display(Name = "Total Marks")]
    public int TotalMarks { get; set; } = 100;

    [Display(Name = "Passing Marks")]
    public int PassingMarks { get; set; } = 33;

    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    public IEnumerable<SelectListItem> SubjectTypeOptions { get; set; } = new List<SelectListItem>();
}

public class AssignSubjectViewModel
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    
    [Required]
    public int SubjectId { get; set; }
    
    public int? TeacherId { get; set; }
    
    [Display(Name = "Schedule Day")]
    public string ScheduleDay { get; set; } = string.Empty;
    
    [Display(Name = "Start Time")]
    [DataType(DataType.Time)]
    public TimeSpan? StartTime { get; set; }
    
    [Display(Name = "End Time")]
    [DataType(DataType.Time)]
    public TimeSpan? EndTime { get; set; }
    
    public IEnumerable<SelectListItem> SubjectOptions { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> TeacherOptions { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> DayOptions { get; set; } = new List<SelectListItem>();
}

public class ClassStatisticsViewModel
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int TotalSubjects { get; set; }
    public int MaxCapacity { get; set; }
    public int CurrentStrength { get; set; }
    public int AvailableSeats { get; set; }
    public double AverageAttendance { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string ClassTeacher { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

public class StudentSimpleViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}