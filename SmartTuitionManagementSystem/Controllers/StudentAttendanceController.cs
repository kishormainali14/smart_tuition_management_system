using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using SmartTuitionManagementSystem.Models;
using SmartTuitionManagementSystem.Services.Interface;

namespace SmartTuitionManagementSystem.Controllers;

public class StudentAttendanceController(IAttendanceService attendanceService) : Controller
{
    private static DateTime ToUtc(DateTime date) =>
        date.Kind switch
        {
            DateTimeKind.Utc => date,
            DateTimeKind.Local => date.ToUniversalTime(),
            _ => DateTime.SpecifyKind(date, DateTimeKind.Utc)
        };
    
    // GET: Attendance/StudentAttendance
    public async Task<IActionResult> StudentAttendance(DateTime? selectedDate, int? selectedClassId)
    {
        var classList = await attendanceService.GetClassListAsync();
        
        if (!classList.Any())
        {
            ViewBag.ErrorMessage = "No classes found. Please add classes first.";
            return View(new DailyAttendanceViewModel());
        }
        
        var date = ToUtc(selectedDate ?? DateTime.UtcNow.Date);
        
        if (!selectedClassId.HasValue || selectedClassId.Value <= 0)
        {
            var emptyModel = new DailyAttendanceViewModel
            {
                SelectedDate = date,
                ClassList = classList
            };
            return View(emptyModel);
        }
        
        var viewModel = await attendanceService.GetAttendanceSheetAsync(date, selectedClassId.Value);
        return View(viewModel);
    }
    
    // POST: Attendance/SaveAttendance
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveAttendance(DailyAttendanceViewModel model)
    {
        model.SelectedDate = ToUtc(model.SelectedDate);

        if (!ModelState.IsValid)
        {
            model.ClassList = await attendanceService.GetClassListAsync();
            var errors = string.Join("; ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            TempData["ErrorMessage"] = $"Validation failed: {errors}";
            return RedirectToAction(nameof(StudentAttendance), new { selectedDate = model.SelectedDate, selectedClassId = model.SelectedClassId });
        }
        
        int userId = 1;
        
        var (success, wasUpdate) = await attendanceService.SaveAttendanceAsync(model, userId);
        
        if (success)
        {
            if (wasUpdate)
                TempData["SuccessMessage"] = "Attendance has been updated successfully.";
            else
                TempData["SuccessMessage"] = "Attendance has been saved successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to save attendance. Please try again.";
        }
        
        return RedirectToAction(nameof(StudentAttendance), new { selectedDate = model.SelectedDate, selectedClassId = model.SelectedClassId });
    }
    
    // GET: Attendance/StudentHistory/5
    public async Task<IActionResult> StudentHistory(int id)
    {
        var history = await attendanceService.GetStudentHistoryAsync(id);
        
        if (history == null)
            return NotFound();
        
        return View(history);
    }
    
    // GET: Attendance/CopyFromPrevious
    [HttpGet]
    public async Task<IActionResult> CopyFromPrevious(DateTime date, int classId)
    {
        var viewModel = await attendanceService.CopyFromPreviousDayAsync(ToUtc(date), classId);
        return Json(viewModel.Students);
    }
    
    // GET: Attendance/AttendanceReport
    public async Task<IActionResult> AttendanceReport(int? selectedClassId, DateTime? fromDate, DateTime? toDate)
    {
        if (!fromDate.HasValue)
            fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        if (!toDate.HasValue)
            toDate = DateTime.UtcNow.Date;

        var model = await attendanceService.GetAttendanceReportAsync(selectedClassId, ToUtc(fromDate.Value), ToUtc(toDate.Value));
        return View(model);
    }

    // GET: Attendance/CheckStatus
    public async Task<IActionResult> CheckStatus(DateTime selectedDate, int selectedClassId)
    {
        var isMarked = await attendanceService.IsAttendanceMarkedAsync(ToUtc(selectedDate), selectedClassId);
        return Json(new { isMarked, date = selectedDate });
    }
}