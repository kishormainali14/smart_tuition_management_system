// Models/TeacherAttendanceViewModels.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartTuitionManagementSystem.Models;

// ================================
// Daily Attendance ViewModels
// ================================

/// <summary>
/// ViewModel for daily teacher attendance marking page (Index.cshtml)
/// </summary>
public class DailyTeacherAttendanceViewModel
{
    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Attendance Date")]
    public DateTime SelectedDate { get; set; } = DateTime.Today;

    public List<TeacherAttendanceRow> Teachers { get; set; } = new();

    public string SuccessMessage { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;

    // Summary Statistics
    public int TotalTeachers => Teachers.Count;
    public int PresentCount => Teachers.Count(t => t.Status == AttendanceStatus.Present.ToString());
    public int AbsentCount => Teachers.Count(t => t.Status == AttendanceStatus.Absent.ToString());
    public int LateCount => Teachers.Count(t => t.Status == AttendanceStatus.Late.ToString());
    public int HalfDayCount => Teachers.Count(t => t.Status == AttendanceStatus.HalfDay.ToString());
    public int LeaveCount => Teachers.Count(t => t.Status == AttendanceStatus.Leave.ToString());
    
    public double AttendancePercentage => TotalTeachers > 0 ? (double)PresentCount / TotalTeachers * 100 : 0;

    // Status options for dropdown
    public List<SelectListItem> StatusOptions => new()
    {
        new SelectListItem { Value = "Present", Text = "Present" },
        new SelectListItem { Value = "Absent", Text = "Absent" },
        new SelectListItem { Value = "Late", Text = "Late" },
        new SelectListItem { Value = "HalfDay", Text = "Half Day" },
        new SelectListItem { Value = "Leave", Text = "Leave" }
    };
}

/// <summary>
/// Represents a single teacher's attendance row in the daily attendance form
/// </summary>
public class TeacherAttendanceRow
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    
    [Display(Name = "Status")]
    public string Status { get; set; } = "Present";
    
    [Display(Name = "Check In Time")]
    [DataType(DataType.Time)]
    public string CheckInTime { get; set; } = string.Empty;
    
    [Display(Name = "Check Out Time")]
    [DataType(DataType.Time)]
    public string CheckOutTime { get; set; } = string.Empty;
    
    [Display(Name = "Remarks")]
    public string Remarks { get; set; } = string.Empty;
    
    public int? AttendanceId { get; set; }
}

// ================================
// Teacher Wise Attendance ViewModels
// ================================

/// <summary>
/// ViewModel for teacher-wise attendance page (TeacherWise.cshtml)
/// </summary>
public class TeacherWiseAttendanceViewModel
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    
    public int SelectedYear { get; set; } = DateTime.Now.Year;
    public int SelectedMonth { get; set; } = DateTime.Now.Month;
    public string MonthName { get; set; } = string.Empty;
    
    public int TotalWorkingDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
    public int HalfDays { get; set; }
    public int LeaveDays { get; set; }
    public decimal AttendancePercentage { get; set; }
    public decimal EarnedSalary { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetSalary { get; set; }
    
    public List<DailyTeacherAttendanceRecord> Records { get; set; } = new();
    
    public List<SelectListItem> YearOptions => GetYearOptions();
    public List<SelectListItem> MonthOptions => GetMonthOptions();
    
    private List<SelectListItem> GetYearOptions()
    {
        var years = new List<SelectListItem>();
        for (int y = DateTime.Now.Year - 2; y <= DateTime.Now.Year + 1; y++)
        {
            years.Add(new SelectListItem { Value = y.ToString(), Text = y.ToString() });
        }
        return years;
    }
    
    private List<SelectListItem> GetMonthOptions()
    {
        return new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "January" },
            new SelectListItem { Value = "2", Text = "February" },
            new SelectListItem { Value = "3", Text = "March" },
            new SelectListItem { Value = "4", Text = "April" },
            new SelectListItem { Value = "5", Text = "May" },
            new SelectListItem { Value = "6", Text = "June" },
            new SelectListItem { Value = "7", Text = "July" },
            new SelectListItem { Value = "8", Text = "August" },
            new SelectListItem { Value = "9", Text = "September" },
            new SelectListItem { Value = "10", Text = "October" },
            new SelectListItem { Value = "11", Text = "November" },
            new SelectListItem { Value = "12", Text = "December" }
        };
    }
}

/// <summary>
/// Represents a single day's attendance record for a teacher
/// </summary>
public class DailyTeacherAttendanceRecord
{
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusBadge => GetStatusBadge(Status);
    public string CheckInTime { get; set; } = string.Empty;
    public string CheckOutTime { get; set; } = string.Empty;
    public string WorkingHours { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public int? AttendanceId { get; set; }
    public bool CanEdit { get; set; } = true;
    
    private string GetStatusBadge(string status)
    {
        return status switch
        {
            "Present" => "success",
            "Absent" => "danger",
            "Late" => "warning",
            "HalfDay" => "info",
            "Leave" => "secondary",
            _ => "light"
        };
    }
}

// ================================
// Monthly Report ViewModels
// ================================

/// <summary>
/// ViewModel for monthly attendance report (MonthlyReport.cshtml)
/// </summary>
public class MonthlyReportViewModel
{
    public int SelectedYear { get; set; } = DateTime.Now.Year;
    public int SelectedMonth { get; set; } = DateTime.Now.Month;
    public string MonthName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<TeacherMonthlySummary> Summaries { get; set; } = new();
    
    public int TotalTeachers => Summaries.Count;
    public int TotalPresent => Summaries.Sum(s => s.PresentDays);
    public int TotalAbsent => Summaries.Sum(s => s.AbsentDays);
    public int TotalLate => Summaries.Sum(s => s.LateDays);
    public int TotalHalfDay => Summaries.Sum(s => s.HalfDays);
    public int TotalLeave => Summaries.Sum(s => s.LeaveDays);
    public decimal TotalEarnedSalary => Summaries.Sum(s => s.EarnedSalary);
    public decimal TotalDeductions => Summaries.Sum(s => s.Deductions);
    public decimal TotalNetSalary => Summaries.Sum(s => s.NetSalary);
    
    public List<SelectListItem> YearOptions => GetYearOptions();
    public List<SelectListItem> MonthOptions => GetMonthOptions();
    
    private List<SelectListItem> GetYearOptions()
    {
        var years = new List<SelectListItem>();
        for (int y = DateTime.Now.Year - 3; y <= DateTime.Now.Year + 1; y++)
        {
            years.Add(new SelectListItem { Value = y.ToString(), Text = y.ToString() });
        }
        return years;
    }
    
    private List<SelectListItem> GetMonthOptions()
    {
        return new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "January" },
            new SelectListItem { Value = "2", Text = "February" },
            new SelectListItem { Value = "3", Text = "March" },
            new SelectListItem { Value = "4", Text = "April" },
            new SelectListItem { Value = "5", Text = "May" },
            new SelectListItem { Value = "6", Text = "June" },
            new SelectListItem { Value = "7", Text = "July" },
            new SelectListItem { Value = "8", Text = "August" },
            new SelectListItem { Value = "9", Text = "September" },
            new SelectListItem { Value = "10", Text = "October" },
            new SelectListItem { Value = "11", Text = "November" },
            new SelectListItem { Value = "12", Text = "December" }
        };
    }
}

/// <summary>
/// Monthly summary for a single teacher
/// </summary>
public class TeacherMonthlySummary
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
    public int HalfDays { get; set; }
    public int LeaveDays { get; set; }
    public int TotalWorkingDays { get; set; }
    public decimal AttendancePercentage { get; set; }
    public decimal EarnedSalary { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetSalary { get; set; }
    
    public string AttendancePercentageFormatted => $"{AttendancePercentage:F1}%";
    public string NetSalaryFormatted => NetSalary.ToString("C");
}

// ================================
// Salary Calculation ViewModels
// ================================

/// <summary>
/// ViewModel for salary calculation page (SalaryCalculation.cshtml)
/// </summary>
public class SalaryCalculationViewModel
{
    public int SelectedYear { get; set; } = DateTime.Now.Year;
    public int SelectedMonth { get; set; } = DateTime.Now.Month;
    public string MonthName { get; set; } = string.Empty;
    public decimal TotalSalaryExpense { get; set; }
    public List<SalarySummary> Salaries { get; set; } = new();
    
    public List<SelectListItem> YearOptions => GetYearOptions();
    public List<SelectListItem> MonthOptions => GetMonthOptions();
    
    private List<SelectListItem> GetYearOptions()
    {
        var years = new List<SelectListItem>();
        for (int y = DateTime.Now.Year - 2; y <= DateTime.Now.Year + 1; y++)
        {
            years.Add(new SelectListItem { Value = y.ToString(), Text = y.ToString() });
        }
        return years;
    }
    
    private List<SelectListItem> GetMonthOptions()
    {
        return new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "January" },
            new SelectListItem { Value = "2", Text = "February" },
            new SelectListItem { Value = "3", Text = "March" },
            new SelectListItem { Value = "4", Text = "April" },
            new SelectListItem { Value = "5", Text = "May" },
            new SelectListItem { Value = "6", Text = "June" },
            new SelectListItem { Value = "7", Text = "July" },
            new SelectListItem { Value = "8", Text = "August" },
            new SelectListItem { Value = "9", Text = "September" },
            new SelectListItem { Value = "10", Text = "October" },
            new SelectListItem { Value = "11", Text = "November" },
            new SelectListItem { Value = "12", Text = "December" }
        };
    }
}

/// <summary>
/// Salary summary for a single teacher
/// </summary>
public class SalarySummary
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
    public int HalfDays { get; set; }
    public int LeaveDays { get; set; }
    public decimal EarnedSalary { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetSalary { get; set; }
    
    public string NetSalaryFormatted => NetSalary.ToString("C");
}

// ================================
// Edit Attendance ViewModels
// ================================

/// <summary>
/// ViewModel for editing a single attendance record (Edit.cshtml)
/// </summary>
public class EditAttendanceViewModel
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Date")]
    public DateTime Date { get; set; }
    
    [Required]
    [Display(Name = "Status")]
    public AttendanceStatus Status { get; set; }
    
    [Display(Name = "Check In Time")]
    [DataType(DataType.Time)]
    public string? CheckInTime { get; set; }
    
    [Display(Name = "Check Out Time")]
    [DataType(DataType.Time)]
    public string? CheckOutTime { get; set; }
    
    [Display(Name = "Remarks")]
    public string? Remarks { get; set; }
    
    [Display(Name = "Is Approved")]
    public bool IsApproved { get; set; }
    
    public List<SelectListItem> StatusOptions => new()
    {
        new SelectListItem { Value = "1", Text = "Present" },
        new SelectListItem { Value = "2", Text = "Absent" },
        new SelectListItem { Value = "3", Text = "Late" },
        new SelectListItem { Value = "4", Text = "Half Day" },
        new SelectListItem { Value = "5", Text = "Leave" }
    };
}

// ================================
// Service Layer DTOs
// ================================

/// <summary>
/// Detailed salary calculation result for a single teacher
/// </summary>
public class SalaryCalculationResult
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public decimal PerDayRate { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
    public int HalfDays { get; set; }
    public int LeaveDays { get; set; }
    public decimal EffectivePresentDays { get; set; }
    public decimal EarnedSalary { get; set; }
    public decimal AbsentDeduction { get; set; }
    public decimal LatePenalty { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }
    public DateTime CalculatedOn { get; set; }
}

/// <summary>
/// Attendance statistics for reporting
/// </summary>
public class AttendanceStatistics
{
    public int TotalTeachers { get; set; }
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public int TotalLate { get; set; }
    public int TotalHalfDay { get; set; }
    public int TotalLeave { get; set; }
    public decimal OverallAttendancePercentage { get; set; }
    public Dictionary<string, int> DailyAverage { get; set; } = new();
}