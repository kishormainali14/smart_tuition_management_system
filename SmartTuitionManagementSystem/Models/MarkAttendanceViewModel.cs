using System;
using System.Collections.Generic;

namespace TeacherAttendanceSystem.Models
{
    public enum AttendanceStatus
    {
        Present,
        Absent,
        Late,
        HalfDay,
        Leave
    }

    public class MarkAttendanceViewModel
    {
        public DateTime SelectedDate { get; set; }
        public List<TeacherAttendanceEntry> Teachers { get; set; } = new();
    }

    public class TeacherAttendanceEntry
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public AttendanceStatus? Status { get; set; }
        public string CheckInTime { get; set; } = string.Empty;
        public string CheckOutTime { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public int? AttendanceId { get; set; }
    }

    public class TeacherMonthlySummary
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
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
    }

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
    }
}