using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SmartTuitionManagementSystem.Models;
using SmartTuitionManagementSystem.Services.Interface;

namespace SmartTuitionManagementSystem.Controllers;

public class StudentAttendanceController(IAttendanceService attendanceService) : Controller
{
    // GET: Attendance/StudentAttendance
    public async Task<IActionResult> StudentAttendance(string? grade, DateTime? date)
    {
        var gradeList = await attendanceService.GetGradeListAsync();
        
        if (!gradeList.Any())
        {
            ViewBag.ErrorMessage = "No grades found. Please add students with grades first.";
            return View(new DailyAttendanceViewModel());
        }
        
        var selectedGrade = grade ?? gradeList.First().Value;
        var selectedDate = date ?? DateTime.Today;
        
        var viewModel = await attendanceService.GetAttendanceSheetAsync(selectedGrade, selectedDate);
        return View(viewModel);
    }
    
    // POST: Attendance/SaveAttendance
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveAttendance(DailyAttendanceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.GradeList = await attendanceService.GetGradeListAsync();
            return View("StudentAttendance", model);
        }
        
        // Get current user ID (adjust based on your auth system)
        int userId = 1; // Replace with actual logged-in user ID
        
        var success = await attendanceService.SaveAttendanceAsync(model, userId);
        
        if (success)
        {
            TempData["SuccessMessage"] = "Attendance saved successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to save attendance. Please try again.";
        }
        
        return RedirectToAction(nameof(StudentAttendance), new { grade = model.SelectedGrade, date = model.SelectedDate });
    }
    
    // GET: Attendance/StudentHistory/5
    public async Task<IActionResult> StudentHistory(int id)
    {
        var history = await attendanceService.GetStudentHistoryAsync(id);
        
        if (history == null)
            return NotFound();
        
        return View(history);
    }
    
    // POST: Attendance/CopyFromPrevious
    [HttpPost]
    public async Task<IActionResult> CopyFromPrevious(string grade, DateTime date)
    {
        var viewModel = await attendanceService.CopyFromPreviousDayAsync(grade, date);
        return Json(viewModel.Students);
    }
    
    // GET: Attendance/CheckStatus
    public async Task<IActionResult> CheckStatus(string grade, DateTime date)
    {
        var isMarked = await attendanceService.IsAttendanceMarkedAsync(grade, date);
        return Json(new { isMarked, date });
    }
}