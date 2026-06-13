using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTuitionManagementSystem.Models;

namespace SmartTuitionManagementSystem.Services.Interface;

public interface IAttendanceService
{
    Task<DailyAttendanceViewModel> GetAttendanceSheetAsync(DateTime date, int classId);
    Task<(bool Success, bool WasUpdate)> SaveAttendanceAsync(DailyAttendanceViewModel model, int markedByUserId);
    Task<List<SelectListItem>> GetClassListAsync();
    Task<StudentAttendanceHistoryViewModel> GetStudentHistoryAsync(int studentId);
    Task<bool> IsAttendanceMarkedAsync(DateTime date, int classId);
    Task<DailyAttendanceViewModel> CopyFromPreviousDayAsync(DateTime date, int classId);
    Task<AttendanceReportViewModel> GetAttendanceReportAsync(int? classId, DateTime? fromDate, DateTime? toDate);
}