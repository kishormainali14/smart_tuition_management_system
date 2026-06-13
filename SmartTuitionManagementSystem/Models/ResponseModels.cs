using System;
using System.Collections.Generic;

namespace SmartTuitionManagementSystem.Models
{
    public class StudentResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string ParentName { get; set; } = string.Empty;
        public string ParentPhone { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class AttendanceResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public string CheckInTime { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }

    public class AttendanceDetailResponse
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public string CheckInTime { get; set; } = string.Empty;
    }

    public class DailyReportResponse
    {
        public DateTime Date { get; set; }
        public int TotalStudents { get; set; }
        public int Present { get; set; }
        public int Late { get; set; }
        public int Absent { get; set; }
        public double AttendancePercentage { get; set; }
        public List<AttendanceDetailResponse> Details { get; set; } = new();
    }

    public class RecentRecordResponse
    {
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public string CheckInTime { get; set; } = string.Empty;
    }

    public class StudentSummaryResponse
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public int TotalDays { get; set; }
        public int Present { get; set; }
        public int Late { get; set; }
        public int Absent { get; set; }
        public double AttendancePercentage { get; set; }
        public double EffectivePercentage { get; set; }
        public List<RecentRecordResponse> RecentRecords { get; set; } = new();
    }
}