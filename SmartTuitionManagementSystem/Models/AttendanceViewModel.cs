using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTuitionManagementSystem.Constants;

namespace SmartTuitionManagementSystem.Models;

public class DailyAttendanceViewModel
{
    [Required]
    [Display(Name = "Select Grade")]
    public string SelectedGrade { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Attendance Date")]
    public DateTime SelectedDate { get; set; } = DateTime.Today;

    public List<SelectListItem> Attendance { get; set; } = new List<SelectListItem>();    public List<SelectListItem> GradeList { get; set; } = new();
    
    public List<StudentAttendanceRow> Students { get; set; } = new();
    
    public string SuccessMessage { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    
    // Summary Statistics
    public int TotalStudents => Students.Count;
    public int PresentCount => Students.Count(s => s.Status == "Present");
    public int AbsentCount => Students.Count(s => s.Status == "Absent");
    public int LateCount => Students.Count(s => s.Status == "Late");
    public int ExcusedCount => Students.Count(s => s.Status == "Excused");
    public double AttendancePercentage => TotalStudents > 0 ? (double)PresentCount / TotalStudents * 100 : 0;
}

public class StudentAttendanceRow
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = "Present";
    public string Remarks { get; set; } = string.Empty;
    public int? AttendanceId { get; set; }
}

public class StudentAttendanceHistoryViewModel
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ParentPhone { get; set; } = string.Empty;
    public int TotalDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
    public int ExcusedDays { get; set; }
    public double AttendancePercentage { get; set; }
    public List<DailyAttendanceRecord> Records { get; set; } = new();
}

public class DailyAttendanceRecord
{
    public DateTime Date { get; set; }  
    public string Status { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
}