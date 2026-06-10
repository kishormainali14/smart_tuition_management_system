// Services/ITeacherAttendanceService.cs

using SmartTuitionManagementSystem.Entities;
using SmartTuitionManagementSystem.Models;

namespace SmartTuitionManagementSystem.Services;

/// <summary>
/// Service interface for Teacher Attendance management
/// </summary>
public interface ITeacherAttendanceService
{
    // ================================
    // Attendance Operations (CRUD)
    // ================================

    /// <summary>
    /// Gets daily attendance view model for a specific date
    /// </summary>
    Task<DailyTeacherAttendanceViewModel> GetDailyAttendanceAsync(DateTime date);

    /// <summary>
    /// Marks or updates attendance for a single teacher
    /// </summary>
    Task<bool> MarkAttendanceAsync(int teacherId, DateTime date, AttendanceStatus status, 
        string? checkInTime, string? checkOutTime, string? remarks);

    /// <summary>
    /// Marks attendance for multiple teachers at once (bulk operation)
    /// </summary>
    Task<bool> MarkBulkAttendanceAsync(List<TeacherAttendanceRow> attendances, DateTime date);

    /// <summary>
    /// Gets a single attendance record by ID
    /// </summary>
    Task<TeacherAttendance?> GetAttendanceByIdAsync(int id);

    /// <summary>
    /// Updates an existing attendance record
    /// </summary>
    Task<bool> UpdateAttendanceAsync(EditAttendanceViewModel model);

    /// <summary>
    /// Deletes an attendance record by ID
    /// </summary>
    Task<bool> DeleteAttendanceAsync(int id);

    /// <summary>
    /// Approves or rejects an attendance record
    /// </summary>
    Task<bool> ApproveAttendanceAsync(int id, int approvedBy, bool isApproved, string? rejectionReason = null);

    // ================================
    // Teacher Operations
    // ================================

    /// <summary>
    /// Gets all active teachers (for dropdowns and lists)
    /// </summary>
    Task<List<TeacherEntity>> GetAllActiveTeachersAsync();

    /// <summary>
    /// Gets a single teacher by ID with their attendance records
    /// </summary>
    Task<TeacherEntity?> GetTeacherByIdAsync(int id);

    /// <summary>
    /// Gets teacher-wise attendance view model for monthly report
    /// </summary>
    Task<TeacherWiseAttendanceViewModel> GetTeacherWiseAttendanceAsync(int teacherId, int year, int month);

    // ================================
    // Report Operations
    // ================================

    /// <summary>
    /// Gets all attendance records for a specific teacher in a given month
    /// </summary>
    Task<List<TeacherAttendance>> GetTeacherAttendanceByMonthAsync(int teacherId, int year, int month);

    /// <summary>
    /// Gets attendance records for a specific teacher by date range
    /// </summary>
    Task<List<TeacherAttendance>> GetTeacherAttendanceByDateRangeAsync(int teacherId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets monthly summary for a specific teacher
    /// </summary>
    Task<TeacherMonthlySummary?> GetTeacherMonthlySummaryAsync(int teacherId, int year, int month);

    /// <summary>
    /// Gets complete monthly report for all teachers
    /// </summary>
    Task<MonthlyReportViewModel> GetMonthlyReportAsync(int year, int month);

    /// <summary>
    /// Gets attendance statistics for a date range
    /// </summary>
    Task<AttendanceStatistics> GetAttendanceStatisticsAsync(DateTime startDate, DateTime endDate);

    // ================================
    // Salary Operations
    // ================================

    /// <summary>
    /// Calculates salary for a specific teacher for a given month
    /// </summary>
    Task<SalaryCalculationResult> CalculateTeacherSalaryAsync(int teacherId, int year, int month);

    /// <summary>
    /// Calculates salary for all teachers for a given month
    /// </summary>
    Task<SalaryCalculationViewModel> CalculateAllSalariesAsync(int year, int month);

    // ================================
    // Validation & Helper Operations
    // ================================

    /// <summary>
    /// Checks if attendance already exists for a teacher on a specific date
    /// </summary>
    Task<bool> AttendanceExistsAsync(int teacherId, DateTime date);

    /// <summary>
    /// Gets existing attendance ID for a teacher on a specific date
    /// </summary>
    Task<int?> GetAttendanceIdAsync(int teacherId, DateTime date);

    /// <summary>
    /// Validates if check-out time is after check-in time
    /// </summary>
    bool IsValidTimeRange(DateTime? checkIn, DateTime? checkOut);

    /// <summary>
    /// Calculates total working hours from check-in and check-out
    /// </summary>
    decimal? CalculateTotalWorkingHours(DateTime? checkIn, DateTime? checkOut);

    /// <summary>
    /// Gets working days count for a specific month (excluding weekends if configured)
    /// </summary>
    int GetWorkingDaysCount(int year, int month);
}