// Controllers/TeacherAttendanceController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTuitionManagementSystem.Models;
using SmartTuitionManagementSystem.Services;

namespace SmartTuitionManagementSystem.Controllers;

public class TeacherAttendanceController : Controller
{
    private readonly ITeacherAttendanceService _attendanceService;
    private readonly ILogger<TeacherAttendanceController> _logger;

    public TeacherAttendanceController(
        ITeacherAttendanceService attendanceService,
        ILogger<TeacherAttendanceController> logger)
    {
        _attendanceService = attendanceService;
        _logger = logger;
    }

    // ================================
    // Daily Attendance
    // ================================

    /// <summary>
    /// GET: TeacherAttendance/Index
    /// Displays daily attendance marking page
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(DateTime? date)
    {
        try
        {
            var selectedDate = date ?? DateTime.Today;
            var viewModel = await _attendanceService.GetDailyAttendanceAsync(selectedDate);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading daily attendance page");
            TempData["ErrorMessage"] = "Failed to load attendance data";
            return RedirectToAction("Index", "Home");
        }
    }

    /// <summary>
    /// POST: TeacherAttendance/MarkAttendance
    /// Marks attendance for a single teacher
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAttendance(int teacherId, DateTime date, string status, 
        string? checkInTime, string? checkOutTime, string? remarks)
    {
        try
        {
            if (!Enum.TryParse<AttendanceStatus>(status, out var attendanceStatus))
            {
                TempData["ErrorMessage"] = "Invalid attendance status";
                return RedirectToAction("Index", new { date });
            }

            var result = await _attendanceService.MarkAttendanceAsync(
                teacherId, date, attendanceStatus, checkInTime, checkOutTime, remarks);

            if (result)
            {
                TempData["SuccessMessage"] = "Attendance marked successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to mark attendance";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking attendance for teacher {TeacherId}", teacherId);
            TempData["ErrorMessage"] = "An error occurred while marking attendance";
        }

        return RedirectToAction("Index", new { date });
    }

    /// <summary>
    /// POST: TeacherAttendance/MarkBulkAttendance
    /// Marks attendance for multiple teachers at once
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkBulkAttendance(DailyTeacherAttendanceViewModel model)
    {
        try
        {
            if (model.Teachers == null || !model.Teachers.Any())
            {
                TempData["ErrorMessage"] = "No teachers data provided";
                return RedirectToAction("Index", new { date = model.SelectedDate });
            }

            var result = await _attendanceService.MarkBulkAttendanceAsync(model.Teachers, model.SelectedDate);

            if (result)
            {
                TempData["SuccessMessage"] = $"Attendance marked for {model.Teachers.Count} teachers successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to mark attendance for some teachers";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk marking attendance");
            TempData["ErrorMessage"] = "An error occurred while marking attendance";
        }

        return RedirectToAction("Index", new { date = model.SelectedDate });
    }

    // ================================
    // Teacher Wise Attendance
    // ================================

    /// <summary>
    /// GET: TeacherAttendance/TeacherWise
    /// Displays attendance summary for a specific teacher
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> TeacherWise(int? id, int? year, int? month)
    {
        try
        {
            if (id == null)
            {
                // Show teacher selection page
                var teachers = await _attendanceService.GetAllActiveTeachersAsync();
                ViewBag.Teachers = new SelectList(teachers, "Id", "FullName");
                return View("SelectTeacher");
            }

            var selectedYear = year ?? DateTime.Now.Year;
            var selectedMonth = month ?? DateTime.Now.Month;

            var viewModel = await _attendanceService.GetTeacherWiseAttendanceAsync(id.Value, selectedYear, selectedMonth);
            
            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "Teacher not found";
                return RedirectToAction("TeacherWise");
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading teacher-wise attendance");
            TempData["ErrorMessage"] = "Failed to load teacher attendance data";
            return RedirectToAction("Index");
        }
    }

    // ================================
    // Monthly Report
    // ================================

    /// <summary>
    /// GET: TeacherAttendance/MonthlyReport
    /// Displays monthly attendance report for all teachers
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> MonthlyReport(int? year, int? month)
    {
        try
        {
            var selectedYear = year ?? DateTime.Now.Year;
            var selectedMonth = month ?? DateTime.Now.Month;

            var viewModel = await _attendanceService.GetMonthlyReportAsync(selectedYear, selectedMonth);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading monthly report");
            TempData["ErrorMessage"] = "Failed to load monthly report";
            return RedirectToAction("Index");
        }
    }

    // ================================
    // Salary Calculation
    // ================================

    /// <summary>
    /// GET: TeacherAttendance/SalaryCalculation
    /// Displays salary calculation for all teachers
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> SalaryCalculation(int? year, int? month)
    {
        try
        {
            var selectedYear = year ?? DateTime.Now.Year;
            var selectedMonth = month ?? DateTime.Now.Month;

            var viewModel = await _attendanceService.CalculateAllSalariesAsync(selectedYear, selectedMonth);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading salary calculation");
            TempData["ErrorMessage"] = "Failed to load salary data";
            return RedirectToAction("Index");
        }
    }

    // ================================
    // Edit Attendance
    // ================================

    /// <summary>
    /// GET: TeacherAttendance/Edit/5
    /// Displays edit form for a single attendance record
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var attendance = await _attendanceService.GetAttendanceByIdAsync(id);
            if (attendance == null)
            {
                TempData["ErrorMessage"] = "Attendance record not found";
                return RedirectToAction("TeacherWise");
            }

            var teacher = await _attendanceService.GetTeacherByIdAsync(attendance.TeacherId);
            
            var viewModel = new EditAttendanceViewModel
            {
                Id = attendance.Id,
                TeacherId = attendance.TeacherId,
                TeacherName = teacher?.FullName ?? "Unknown",
                Date = attendance.Date,
                Status = attendance.Status,
                CheckInTime = attendance.CheckInTime?.ToString(@"hh\:mm"),
                CheckOutTime = attendance.CheckOutTime?.ToString(@"hh\:mm"),
                Remarks = attendance.Remarks,
                IsApproved = attendance.IsApproved
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit attendance page for ID {Id}", id);
            TempData["ErrorMessage"] = "Failed to load attendance record";
            return RedirectToAction("TeacherWise");
        }
    }

    /// <summary>
    /// POST: TeacherAttendance/Edit/5
    /// Updates an attendance record
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditAttendanceViewModel model)
    {
        if (id != model.Id)
        {
            TempData["ErrorMessage"] = "Attendance record mismatch";
            return RedirectToAction("TeacherWise");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await _attendanceService.UpdateAttendanceAsync(model);
            
            if (result)
            {
                TempData["SuccessMessage"] = "Attendance updated successfully!";
                return RedirectToAction("TeacherWise", new { id = model.TeacherId });
            }
            
            TempData["ErrorMessage"] = "Failed to update attendance";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating attendance ID {Id}", id);
            TempData["ErrorMessage"] = "An error occurred while updating attendance";
        }

        return View(model);
    }

    /// <summary>
    /// POST: TeacherAttendance/Delete/5
    /// Deletes an attendance record
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int teacherId)
    {
        try
        {
            var result = await _attendanceService.DeleteAttendanceAsync(id);
            
            if (result)
            {
                TempData["SuccessMessage"] = "Attendance record deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete attendance record";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting attendance ID {Id}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting attendance";
        }

        return RedirectToAction("TeacherWise", new { id = teacherId });
    }

    // ================================
    // Approval Actions
    // ================================

    /// <summary>
    /// POST: TeacherAttendance/Approve/5
    /// Approves an attendance record
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, int teacherId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var result = await _attendanceService.ApproveAttendanceAsync(id, currentUserId, true);
            
            if (result)
            {
                TempData["SuccessMessage"] = "Attendance approved successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve attendance";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving attendance ID {Id}", id);
            TempData["ErrorMessage"] = "An error occurred while approving attendance";
        }

        return RedirectToAction("TeacherWise", new { id = teacherId });
    }

    /// <summary>
    /// POST: TeacherAttendance/Reject/5
    /// Rejects an attendance record
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, int teacherId, string? reason)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var result = await _attendanceService.ApproveAttendanceAsync(id, currentUserId, false, reason);
            
            if (result)
            {
                TempData["SuccessMessage"] = "Attendance rejected successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reject attendance";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting attendance ID {Id}", id);
            TempData["ErrorMessage"] = "An error occurred while rejecting attendance";
        }

        return RedirectToAction("TeacherWise", new { id = teacherId });
    }

    // ================================
    // Export Actions
    // ================================

    /// <summary>
    /// GET: TeacherAttendance/ExportMonthlyReport
    /// Exports monthly report to Excel
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ExportMonthlyReport(int year, int month)
    {
        try
        {
            var viewModel = await _attendanceService.GetMonthlyReportAsync(year, month);
            
            // Build CSV
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Teacher Name,Present,Absent,Late,Half Day,Leave,Total Days,Attendance %,Earned Salary,Deductions,Net Salary");
            
            foreach (var item in viewModel.Summaries)
            {
                csv.AppendLine($"\"{item.TeacherName}\",{item.PresentDays},{item.AbsentDays},{item.LateDays},{item.HalfDays},{item.LeaveDays},{item.TotalWorkingDays},{item.AttendancePercentage:F1}%,{item.EarnedSalary:C},{item.Deductions:C},{item.NetSalary:C}");
            }
            
            csv.AppendLine();
            csv.AppendLine($"Totals,,{viewModel.TotalPresent},{viewModel.TotalAbsent},{viewModel.TotalLate},{viewModel.TotalHalfDay},{viewModel.TotalLeave},,,{viewModel.TotalEarnedSalary:C},{viewModel.TotalDeductions:C},{viewModel.TotalNetSalary:C}");
            
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"MonthlyReport_{viewModel.MonthName}_{year}.csv";
            
            return File(bytes, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting monthly report");
            TempData["ErrorMessage"] = "Failed to export report";
            return RedirectToAction("MonthlyReport", new { year, month });
        }
    }

    /// <summary>
    /// GET: TeacherAttendance/ExportSalaryReport
    /// Exports salary report to Excel
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ExportSalaryReport(int year, int month)
    {
        try
        {
            var viewModel = await _attendanceService.CalculateAllSalariesAsync(year, month);
            
            // Build CSV
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Teacher Name,Base Salary,Present,Absent,Late,Half Day,Leave,Earned Salary,Deductions,Net Salary");
            
            foreach (var item in viewModel.Salaries)
            {
                csv.AppendLine($"\"{item.TeacherName}\",{item.BaseSalary:C},{item.PresentDays},{item.AbsentDays},{item.LateDays},{item.HalfDays},{item.LeaveDays},{item.EarnedSalary:C},{item.Deductions:C},{item.NetSalary:C}");
            }
            
            csv.AppendLine();
            csv.AppendLine($"Total Salary Expense,,{viewModel.TotalSalaryExpense:C}");
            
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"SalaryReport_{viewModel.MonthName}_{year}.csv";
            
            return File(bytes, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting salary report");
            TempData["ErrorMessage"] = "Failed to export salary report";
            return RedirectToAction("SalaryCalculation", new { year, month });
        }
    }

    // ================================
    // Private Helpers
    // ================================

    private int GetCurrentUserId()
    {
        // TODO: Get from actual authentication context
        // For now, return a default value
        var userId = User?.Identity?.Name ?? "1";
        return int.TryParse(userId, out var id) ? id : 1;
    }
}