using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTuitionManagementSystem.Models;

namespace SmartTuitionManagementSystem.Services.Interface;

public interface IAttendanceService
{
    Task<DailyAttendanceViewModel> GetAttendanceSheetAsync(string grade, DateTime date);
    Task<bool> SaveAttendanceAsync(DailyAttendanceViewModel model, int markedByUserId);
    Task<List<SelectListItem>> GetGradeListAsync();
    Task<StudentAttendanceHistoryViewModel> GetStudentHistoryAsync(int studentId);
    Task<bool> IsAttendanceMarkedAsync(string grade, DateTime date);
    Task<DailyAttendanceViewModel> CopyFromPreviousDayAsync(string grade, DateTime date);
}