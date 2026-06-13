using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTuitionManagementSystem.Constants;

namespace SmartTuitionManagementSystem.Models;

public class DailyAttendanceViewModel
{
    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Attendance Date")]
    public DateTime SelectedDate { get; set; } = DateTime.UtcNow.Date;

    public List<SelectListItem> Attendance { get; set; } = new List<SelectListItem>();
    [Required(ErrorMessage = "Please select a class")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid class")]
    [Display(Name = "Class")]
    public int SelectedClassId { get; set; }
    public List<SelectListItem> ClassList { get; set; } = new();
    
    public List<StudentAttendanceRow> Students { get; set; } = new();
    
    public string SuccessMessage { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    
    // Summary Statistics
    public int TotalStudents => Students.Count;
    public int PresentCount => Students.Count(s => s.Status == "Present");
    public int AbsentCount => Students.Count(s => s.Status == "Absent");
    public int LeaveCount => Students.Count(s => s.Status == "Leave");
    public double AttendancePercentage => TotalStudents > 0 ? (double)PresentCount / TotalStudents * 100 : 0;
}

public class StudentAttendanceRow
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = "Present";
    public string? Remarks { get; set; }
    public int? AttendanceId { get; set; }
}

public class StudentAttendanceHistoryViewModel
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ParentPhone { get; set; } = string.Empty;
    public int TotalDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LeaveDays { get; set; }
    public double AttendancePercentage { get; set; }
    public List<DailyAttendanceRecord> Records { get; set; } = new();
}

public class DailyAttendanceRecord
{
    public DateTime Date { get; set; }  
    public string Status { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
}

public class AttendanceReportViewModel
{
    [Display(Name = "Class")]
    public int? SelectedClassId { get; set; }

    [Display(Name = "From Date")]
    [DataType(DataType.Date)]
    public DateTime? FromDate { get; set; }

    [Display(Name = "To Date")]
    [DataType(DataType.Date)]
    public DateTime? ToDate { get; set; }

    public List<SelectListItem> ClassList { get; set; } = new();

    public List<AttendanceReportRow> Rows { get; set; } = new();
}

public class AttendanceReportRow
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string RollNumber { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public int TotalLeave { get; set; }
    public int TotalDays => TotalPresent + TotalAbsent + TotalLeave;
    public double AttendancePercentage => TotalDays > 0 ? Math.Round((double)TotalPresent / TotalDays * 100, 2) : 0;
}