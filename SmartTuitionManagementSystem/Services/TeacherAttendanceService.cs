// Services/TeacherAttendanceService.cs
using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Data;
using SmartTuitionManagementSystem.Entities;
using SmartTuitionManagementSystem.Models;

namespace SmartTuitionManagementSystem.Services;

public class TeacherAttendanceService : ITeacherAttendanceService
{
    private readonly ApplicationDbContext _context;

    public TeacherAttendanceService(ApplicationDbContext context)
    {
        _context = context;
    }

    // ================================
    // Attendance Operations
    // ================================

    public async Task<DailyTeacherAttendanceViewModel> GetDailyAttendanceAsync(DateTime date)
    {
        var targetDate = date.Date;
        
        // Get existing attendances for this date
        var attendances = await _context.TeacherAttendances
            .Where(a => a.Date == targetDate)
            .ToDictionaryAsync(a => a.TeacherId);

        // Get all teachers (no IsActive filter - property doesn't exist)
        var teachers = await _context.Teachers
            .OrderBy(t => t.FullName)
            .ToListAsync();

        // Build the view model
        var viewModel = new DailyTeacherAttendanceViewModel
        {
            SelectedDate = targetDate,
            Teachers = teachers.Select(t => new TeacherAttendanceRow
            {
                TeacherId = t.Id,
                TeacherName = t.FullName,
                Email = t.Email,
                Phone = t.PhoneNumber,
                Status = attendances.ContainsKey(t.Id) 
                    ? attendances[t.Id].Status.ToString() 
                    : AttendanceStatus.Present.ToString(),
                CheckInTime = attendances.ContainsKey(t.Id) && attendances[t.Id].CheckInTime.HasValue
                    ? attendances[t.Id].CheckInTime.Value.ToString(@"hh\:mm")
                    : "",
                CheckOutTime = attendances.ContainsKey(t.Id) && attendances[t.Id].CheckOutTime.HasValue
                    ? attendances[t.Id].CheckOutTime.Value.ToString(@"hh\:mm")
                    : "",
                Remarks = attendances.ContainsKey(t.Id) ? attendances[t.Id].Remarks ?? "" : "",
                AttendanceId = attendances.ContainsKey(t.Id) ? attendances[t.Id].Id : null
            }).ToList()
        };

        return viewModel;
    }

    public async Task<bool> MarkAttendanceAsync(int teacherId, DateTime date, AttendanceStatus status, 
        string? checkInTime, string? checkOutTime, string? remarks)
    {
        var targetDate = date.Date;
        
        // Parse time values
        DateTime? parsedCheckIn = null;
        DateTime? parsedCheckOut = null;
        
        if (!string.IsNullOrEmpty(checkInTime))
        {
            var timeParts = checkInTime.Split(':');
            if (timeParts.Length == 2)
            {
                parsedCheckIn = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, 
                    int.Parse(timeParts[0]), int.Parse(timeParts[1]), 0);
            }
        }
        
        if (!string.IsNullOrEmpty(checkOutTime))
        {
            var timeParts = checkOutTime.Split(':');
            if (timeParts.Length == 2)
            {
                parsedCheckOut = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, 
                    int.Parse(timeParts[0]), int.Parse(timeParts[1]), 0);
            }
        }

        var existing = await _context.TeacherAttendances
            .FirstOrDefaultAsync(a => a.TeacherId == teacherId && a.Date == targetDate);

        if (existing != null)
        {
            existing.Status = status;
            existing.CheckInTime = parsedCheckIn;
            existing.CheckOutTime = parsedCheckOut;
            existing.TotalWorkingHours = CalculateTotalWorkingHours(parsedCheckIn, parsedCheckOut);
            existing.Remarks = remarks;
            existing.UpdatedAt = DateTime.Now;
        }
        else
        {
            var attendance = new TeacherAttendance
            {
                TeacherId = teacherId,
                Date = targetDate,
                Status = status,
                CheckInTime = parsedCheckIn,
                CheckOutTime = parsedCheckOut,
                TotalWorkingHours = CalculateTotalWorkingHours(parsedCheckIn, parsedCheckOut),
                Remarks = remarks,
                IsApproved = true,
                CreatedAt = DateTime.Now
            };
            await _context.TeacherAttendances.AddAsync(attendance);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkBulkAttendanceAsync(List<TeacherAttendanceRow> attendances, DateTime date)
    {
        var successCount = 0;
        foreach (var row in attendances)
        {
            var status = Enum.TryParse<AttendanceStatus>(row.Status, out var parsedStatus) 
                ? parsedStatus : AttendanceStatus.Present;
            
            var result = await MarkAttendanceAsync(row.TeacherId, date, status, row.CheckInTime, row.CheckOutTime, row.Remarks);
            if (result) successCount++;
        }
        return successCount == attendances.Count;
    }

    public async Task<TeacherAttendance?> GetAttendanceByIdAsync(int id)
    {
        return await _context.TeacherAttendances
            .Include(a => a.Teacher)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> UpdateAttendanceAsync(EditAttendanceViewModel model)
    {
        var existing = await _context.TeacherAttendances.FindAsync(model.Id);
        if (existing == null) return false;

        DateTime? parsedCheckIn = existing.CheckInTime;
        DateTime? parsedCheckOut = existing.CheckOutTime;
        
        if (!string.IsNullOrEmpty(model.CheckInTime))
        {
            var timeParts = model.CheckInTime.Split(':');
            if (timeParts.Length == 2)
            {
                parsedCheckIn = new DateTime(existing.Date.Year, existing.Date.Month, existing.Date.Day, 
                    int.Parse(timeParts[0]), int.Parse(timeParts[1]), 0);
            }
        }
        
        if (!string.IsNullOrEmpty(model.CheckOutTime))
        {
            var timeParts = model.CheckOutTime.Split(':');
            if (timeParts.Length == 2)
            {
                parsedCheckOut = new DateTime(existing.Date.Year, existing.Date.Month, existing.Date.Day, 
                    int.Parse(timeParts[0]), int.Parse(timeParts[1]), 0);
            }
        }

        existing.Status = model.Status;
        existing.CheckInTime = parsedCheckIn;
        existing.CheckOutTime = parsedCheckOut;
        existing.TotalWorkingHours = CalculateTotalWorkingHours(parsedCheckIn, parsedCheckOut);
        existing.Remarks = model.Remarks;
        existing.IsApproved = model.IsApproved;
        existing.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAttendanceAsync(int id)
    {
        var attendance = await _context.TeacherAttendances.FindAsync(id);
        if (attendance == null) return false;

        _context.TeacherAttendances.Remove(attendance);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ApproveAttendanceAsync(int id, int approvedBy, bool isApproved, string? rejectionReason = null)
    {
        var attendance = await _context.TeacherAttendances.FindAsync(id);
        if (attendance == null) return false;

        attendance.IsApproved = isApproved;
        attendance.ApprovedBy = approvedBy;
        attendance.ApprovedDate = DateTime.Now;
        attendance.RejectionReason = isApproved ? null : rejectionReason;
        attendance.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return true;
    }

    // ================================
    // Teacher Operations
    // ================================

    // Services/TeacherAttendanceService.cs

// Change this method name to match the interface
    public async Task<List<TeacherEntity>> GetAllActiveTeachersAsync() 
    {
        return await _context.Teachers
            .OrderBy(t => t.FullName)
            .ToListAsync();
    }
    public async Task<TeacherEntity?> GetTeacherByIdAsync(int id)
    {
        return await _context.Teachers
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TeacherWiseAttendanceViewModel> GetTeacherWiseAttendanceAsync(int teacherId, int year, int month)
    {
        var teacher = await _context.Teachers.FindAsync(teacherId);
        if (teacher == null) return null!;

        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var totalWorkingDays = GetWorkingDaysCount(year, month);

        var attendances = await _context.TeacherAttendances
            .Where(a => a.TeacherId == teacherId && a.Date >= startDate && a.Date <= endDate)
            .OrderBy(a => a.Date)
            .ToListAsync();

        var present = attendances.Count(a => a.Status == AttendanceStatus.Present);
        var absent = attendances.Count(a => a.Status == AttendanceStatus.Absent);
        var late = attendances.Count(a => a.Status == AttendanceStatus.Late);
        var halfDay = attendances.Count(a => a.Status == AttendanceStatus.HalfDay);
        var leave = attendances.Count(a => a.Status == AttendanceStatus.Leave);

        var effectivePresent = present + (halfDay * 0.5m);
        var perDayRate = totalWorkingDays > 0 ? teacher.Salary / totalWorkingDays : 0;
        var earnedSalary = perDayRate * effectivePresent;
        var absentDeduction = perDayRate * absent;
        var latePenalty = late * (perDayRate * 0.05m);
        var totalDeductions = absentDeduction + latePenalty;
        var netSalary = earnedSalary - totalDeductions;

        var records = new List<DailyTeacherAttendanceRecord>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var attendance = attendances.FirstOrDefault(a => a.Date == date);
            records.Add(new DailyTeacherAttendanceRecord
            {
                Date = date,
                Status = attendance?.Status.ToString() ?? "Not Marked",
                CheckInTime = attendance?.CheckInTime?.ToString(@"hh\:mm") ?? "-",
                CheckOutTime = attendance?.CheckOutTime?.ToString(@"hh\:mm") ?? "-",
                WorkingHours = attendance?.TotalWorkingHours?.ToString("F2") ?? "-",
                Remarks = attendance?.Remarks ?? "",
                AttendanceId = attendance?.Id,
                CanEdit = attendance?.IsApproved != false
            });
        }

        return new TeacherWiseAttendanceViewModel
        {
            TeacherId = teacherId,
            TeacherName = teacher.FullName,
            Email = teacher.Email,
            Phone = teacher.PhoneNumber,
            Qualification = teacher.Qualification,
            Salary = teacher.Salary,
            SelectedYear = year,
            SelectedMonth = month,
            MonthName = startDate.ToString("MMMM yyyy"),
            TotalWorkingDays = totalWorkingDays,
            PresentDays = present,
            AbsentDays = absent,
            LateDays = late,
            HalfDays = halfDay,
            LeaveDays = leave,
            AttendancePercentage = totalWorkingDays > 0 ? (effectivePresent / totalWorkingDays) * 100 : 0,
            EarnedSalary = earnedSalary,
            Deductions = totalDeductions,
            NetSalary = netSalary > 0 ? netSalary : 0,
            Records = records
        };
    }

    // ================================
    // Report Operations
    // ================================

    public async Task<List<TeacherAttendance>> GetTeacherAttendanceByMonthAsync(int teacherId, int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        return await _context.TeacherAttendances
            .Where(a => a.TeacherId == teacherId && a.Date >= startDate && a.Date <= endDate)
            .OrderBy(a => a.Date)
            .ToListAsync();
    }

    public async Task<List<TeacherAttendance>> GetTeacherAttendanceByDateRangeAsync(int teacherId, DateTime startDate, DateTime endDate)
    {
        return await _context.TeacherAttendances
            .Where(a => a.TeacherId == teacherId && a.Date >= startDate && a.Date <= endDate)
            .OrderBy(a => a.Date)
            .ToListAsync();
    }

    public async Task<TeacherMonthlySummary?> GetTeacherMonthlySummaryAsync(int teacherId, int year, int month)
    {
        var teacher = await _context.Teachers.FindAsync(teacherId);
        if (teacher == null) return null;

        var attendances = await GetTeacherAttendanceByMonthAsync(teacherId, year, month);
        var totalWorkingDays = GetWorkingDaysCount(year, month);

        var present = attendances.Count(a => a.Status == AttendanceStatus.Present);
        var absent = attendances.Count(a => a.Status == AttendanceStatus.Absent);
        var late = attendances.Count(a => a.Status == AttendanceStatus.Late);
        var halfDay = attendances.Count(a => a.Status == AttendanceStatus.HalfDay);
        var leave = attendances.Count(a => a.Status == AttendanceStatus.Leave);

        var effectivePresent = present + (halfDay * 0.5m);
        var perDayRate = totalWorkingDays > 0 ? teacher.Salary / totalWorkingDays : 0;
        var earnedSalary = perDayRate * effectivePresent;
        var absentDeduction = perDayRate * absent;
        var latePenalty = late * (perDayRate * 0.05m);
        var totalDeductions = absentDeduction + latePenalty;
        var netSalary = earnedSalary - totalDeductions;

        return new TeacherMonthlySummary
        {
            TeacherId = teacherId,
            TeacherName = teacher.FullName,
            Email = teacher.Email,
            BaseSalary = teacher.Salary,
            PresentDays = present,
            AbsentDays = absent,
            LateDays = late,
            HalfDays = halfDay,
            LeaveDays = leave,
            TotalWorkingDays = totalWorkingDays,
            AttendancePercentage = totalWorkingDays > 0 ? (effectivePresent / totalWorkingDays) * 100 : 0,
            EarnedSalary = earnedSalary,
            Deductions = totalDeductions,
            NetSalary = netSalary > 0 ? netSalary : 0
        };
    }

    public async Task<MonthlyReportViewModel> GetMonthlyReportAsync(int year, int month)
    {
        var teachers = await GetAllActiveTeachersAsync();
        var summaries = new List<TeacherMonthlySummary>();

        foreach (var teacher in teachers)
        {
            var summary = await GetTeacherMonthlySummaryAsync(teacher.Id, year, month);
            if (summary != null)
                summaries.Add(summary);
        }

        var startDate = new DateTime(year, month, 1);
        
        return new MonthlyReportViewModel
        {
            SelectedYear = year,
            SelectedMonth = month,
            MonthName = startDate.ToString("MMMM yyyy"),
            StartDate = startDate,
            EndDate = startDate.AddMonths(1).AddDays(-1),
            Summaries = summaries
        };
    }

    public async Task<AttendanceStatistics> GetAttendanceStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        var attendances = await _context.TeacherAttendances
            .Where(a => a.Date >= startDate && a.Date <= endDate)
            .ToListAsync();

        var teachers = await GetAllActiveTeachersAsync();
        var totalTeachers = teachers.Count;
        var totalDays = (endDate - startDate).Days + 1;

        var totalPresent = attendances.Count(a => a.Status == AttendanceStatus.Present);
        var totalAbsent = attendances.Count(a => a.Status == AttendanceStatus.Absent);
        var totalLate = attendances.Count(a => a.Status == AttendanceStatus.Late);
        var totalHalfDay = attendances.Count(a => a.Status == AttendanceStatus.HalfDay);
        var totalLeave = attendances.Count(a => a.Status == AttendanceStatus.Leave);

        var expectedTotalAttendances = totalTeachers * totalDays;
        var overallAttendancePercentage = expectedTotalAttendances > 0 
            ? (decimal)totalPresent / expectedTotalAttendances * 100 : 0;

        var dailyAverage = new Dictionary<string, int>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var dailyCount = attendances.Count(a => a.Date == date && a.Status == AttendanceStatus.Present);
            dailyAverage.Add(date.ToString("yyyy-MM-dd"), dailyCount);
        }

        return new AttendanceStatistics
        {
            TotalTeachers = totalTeachers,
            TotalPresent = totalPresent,
            TotalAbsent = totalAbsent,
            TotalLate = totalLate,
            TotalHalfDay = totalHalfDay,
            TotalLeave = totalLeave,
            OverallAttendancePercentage = overallAttendancePercentage,
            DailyAverage = dailyAverage
        };
    }

    // ================================
    // Salary Operations
    // ================================

    public async Task<SalaryCalculationResult> CalculateTeacherSalaryAsync(int teacherId, int year, int month)
    {
        var teacher = await _context.Teachers.FindAsync(teacherId);
        if (teacher == null) return null!;

        var attendances = await GetTeacherAttendanceByMonthAsync(teacherId, year, month);
        var totalWorkingDays = GetWorkingDaysCount(year, month);

        var present = attendances.Count(a => a.Status == AttendanceStatus.Present);
        var absent = attendances.Count(a => a.Status == AttendanceStatus.Absent);
        var late = attendances.Count(a => a.Status == AttendanceStatus.Late);
        var halfDay = attendances.Count(a => a.Status == AttendanceStatus.HalfDay);
        var leave = attendances.Count(a => a.Status == AttendanceStatus.Leave);

        var effectivePresentDays = present + (halfDay * 0.5m) + leave;
        var perDayRate = totalWorkingDays > 0 ? teacher.Salary / totalWorkingDays : 0;
        var earnedSalary = perDayRate * effectivePresentDays;
        var absentDeduction = perDayRate * absent;
        var latePenalty = late * (perDayRate * 0.05m);
        var totalDeductions = absentDeduction + latePenalty;
        var netSalary = earnedSalary - totalDeductions;

        return new SalaryCalculationResult
        {
            TeacherId = teacherId,
            TeacherName = teacher.FullName,
            BaseSalary = teacher.Salary,
            PerDayRate = perDayRate,
            PresentDays = present,
            AbsentDays = absent,
            LateDays = late,
            HalfDays = halfDay,
            LeaveDays = leave,
            EffectivePresentDays = effectivePresentDays,
            EarnedSalary = earnedSalary,
            AbsentDeduction = absentDeduction,
            LatePenalty = latePenalty,
            TotalDeductions = totalDeductions,
            NetSalary = netSalary > 0 ? netSalary : 0,
            CalculatedOn = DateTime.Now
        };
    }

    public async Task<SalaryCalculationViewModel> CalculateAllSalariesAsync(int year, int month)
    {
        var teachers = await GetAllActiveTeachersAsync();
        var salaries = new List<SalarySummary>();

        foreach (var teacher in teachers)
        {
            var result = await CalculateTeacherSalaryAsync(teacher.Id, year, month);
            if (result != null)
            {
                salaries.Add(new SalarySummary
                {
                    TeacherId = teacher.Id,
                    TeacherName = teacher.FullName,
                    BaseSalary = teacher.Salary,
                    PresentDays = result.PresentDays,
                    AbsentDays = result.AbsentDays,
                    LateDays = result.LateDays,
                    HalfDays = result.HalfDays,
                    LeaveDays = result.LeaveDays,
                    EarnedSalary = result.EarnedSalary,
                    Deductions = result.TotalDeductions,
                    NetSalary = result.NetSalary
                });
            }
        }

        var startDate = new DateTime(year, month, 1);
        
        return new SalaryCalculationViewModel
        {
            SelectedYear = year,
            SelectedMonth = month,
            MonthName = startDate.ToString("MMMM yyyy"),
            TotalSalaryExpense = salaries.Sum(s => s.NetSalary),
            Salaries = salaries
        };
    }

    // ================================
    // Validation & Helper Operations
    // ================================

    public async Task<bool> AttendanceExistsAsync(int teacherId, DateTime date)
    {
        return await _context.TeacherAttendances
            .AnyAsync(a => a.TeacherId == teacherId && a.Date == date.Date);
    }

    public async Task<int?> GetAttendanceIdAsync(int teacherId, DateTime date)
    {
        var attendance = await _context.TeacherAttendances
            .FirstOrDefaultAsync(a => a.TeacherId == teacherId && a.Date == date.Date);
        return attendance?.Id;
    }

    public bool IsValidTimeRange(DateTime? checkIn, DateTime? checkOut)
    {
        if (!checkIn.HasValue || !checkOut.HasValue) return true;
        return checkOut.Value > checkIn.Value;
    }

    public decimal? CalculateTotalWorkingHours(DateTime? checkIn, DateTime? checkOut)
    {
        if (!checkIn.HasValue || !checkOut.HasValue) return null;
        if (checkOut.Value <= checkIn.Value) return null;
        
        var hours = (checkOut.Value - checkIn.Value).TotalHours;
        return Math.Round((decimal)hours, 2);
    }

    public int GetWorkingDaysCount(int year, int month)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var workingDays = 0;
        
        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(year, month, day);
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
            {
                workingDays++;
            }
        }
        
        return workingDays;
    }
}