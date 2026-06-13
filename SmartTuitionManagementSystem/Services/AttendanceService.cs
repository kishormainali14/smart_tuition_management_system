using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Data;
using SmartTuitionManagementSystem.Entities;
using SmartTuitionManagementSystem.Models;
using SmartTuitionManagementSystem.Services.Interface;

namespace SmartTuitionManagementSystem.Services;

public class AttendanceService : IAttendanceService
{
    private readonly ApplicationDbContext _context;
    
    public AttendanceService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    private static DateTime ToUtc(DateTime date) =>
        date.Kind switch
        {
            DateTimeKind.Utc => date,
            DateTimeKind.Local => date.ToUniversalTime(),
            _ => DateTime.SpecifyKind(date, DateTimeKind.Utc)
        };

    public async Task<DailyAttendanceViewModel> GetAttendanceSheetAsync(DateTime date, int classId)
    {
        date = ToUtc(date);
        var viewModel = new DailyAttendanceViewModel
        {
            SelectedDate = date,
            SelectedClassId = classId,
            ClassList = await GetClassListAsync()
        };
        
        // Get students filtered by classId
        IQueryable<StudentEntity> query = _context.Students.Where(s => s.IsActive && s.ClassId == classId);
        
        var students = await query
            .OrderBy(s => s.FullName)
            .ToListAsync();
        
        // Get existing attendance for this date
        var existingAttendance = await _context.Attendances
            .Where(a => a.AttendanceDate.Date == date.Date)
            .ToDictionaryAsync(a => a.StudentId);
        
        foreach (var student in students)
        {
            var existing = existingAttendance.ContainsKey(student.Id) 
                ? existingAttendance[student.Id] : null;
            
            viewModel.Students.Add(new StudentAttendanceRow
            {
                StudentId = student.Id,
                FullName = student.FullName,
                Email = student.Email,
                Status = existing?.Status ?? "Present",
                Remarks = existing?.Remarks ?? string.Empty,
                AttendanceId = existing?.Id
            });
        }
        
        return viewModel;
    }
    
    public async Task<(bool Success, bool WasUpdate)> SaveAttendanceAsync(DailyAttendanceViewModel model, int markedByUserId)
    {
        try
        {
            var utcDate = ToUtc(model.SelectedDate);
            var newCount = 0;
            var updateCount = 0;
            
            foreach (var studentRow in model.Students)
            {
                var existing = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.StudentId == studentRow.StudentId && 
                                              a.AttendanceDate.Date == utcDate.Date);
                
                if (existing != null)
                {
                    existing.Status = studentRow.Status;
                    existing.Remarks = studentRow.Remarks ?? string.Empty;
                    existing.MarkedAt = DateTime.UtcNow;
                    existing.MarkedBy = markedByUserId;
                    updateCount++;
                }
                else
                {
                    var attendance = new StudentAttendanceEntity
                    {
                        StudentId = studentRow.StudentId,
                        AttendanceDate = utcDate,
                        Status = studentRow.Status,
                        Remarks = studentRow.Remarks ?? string.Empty,
                        MarkedAt = DateTime.UtcNow,
                        MarkedBy = markedByUserId
                    };
                    await _context.Attendances.AddAsync(attendance);
                    newCount++;
                }
            }
            
            await _context.SaveChangesAsync();
            return (true, updateCount > 0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving attendance: {ex.Message}");
            return (false, false);
        }
    }
    
    public async Task<List<SelectListItem>> GetClassListAsync()
    {
        var classes = await _context.Classes
            .Where(c => c.IsActive)
            .OrderBy(c => c.ClassName)
            .ToListAsync();
        
        return classes.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = $"{c.ClassName} {(string.IsNullOrEmpty(c.Section) ? "" : "- " + c.Section)}".Trim()
        }).ToList();
    }
    
    public async Task<StudentAttendanceHistoryViewModel> GetStudentHistoryAsync(int studentId)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == studentId);
        
        if (student == null) return null;
        
        var attendances = await _context.Attendances
            .Where(a => a.StudentId == studentId)
            .OrderByDescending(a => a.AttendanceDate)
            .ToListAsync();
        
        var totalDays = attendances.Count;
        var presentDays = attendances.Count(a => a.Status == "Present");
        var absentDays = attendances.Count(a => a.Status == "Absent");
        var leaveDays = attendances.Count(a => a.Status == "Leave");
        
        return new StudentAttendanceHistoryViewModel
        {
            StudentId = student.Id,
            StudentName = student.FullName,
            Email = student.Email,
            ParentPhone = student.ParentPhone,
            TotalDays = totalDays,
            PresentDays = presentDays,
            AbsentDays = absentDays,
            LeaveDays = leaveDays,
            AttendancePercentage = totalDays > 0 ? (double)presentDays / totalDays * 100 : 0,
            Records = attendances.Select(a => new DailyAttendanceRecord
            {
                Date = a.AttendanceDate,
                Status = a.Status,
                Remarks = a.Remarks
            }).ToList()
        };
    }
    
    public async Task<bool> IsAttendanceMarkedAsync(DateTime date, int classId)
    {
        date = ToUtc(date);
        
        var studentIds = await _context.Students
            .Where(s => s.IsActive && s.ClassId == classId)
            .Select(s => s.Id)
            .ToListAsync();
        
        var count = await _context.Attendances
            .CountAsync(a => a.AttendanceDate.Date == date.Date && 
                            studentIds.Contains(a.StudentId));
        
        return count > 0;
    }
    
    public async Task<AttendanceReportViewModel> GetAttendanceReportAsync(int? classId, DateTime? fromDate, DateTime? toDate)
    {
        fromDate = fromDate.HasValue ? ToUtc(fromDate.Value) : null;
        toDate = toDate.HasValue ? ToUtc(toDate.Value) : null;

        var viewModel = new AttendanceReportViewModel
        {
            SelectedClassId = classId,
            FromDate = fromDate,
            ToDate = toDate,
            ClassList = await GetClassListAsync()
        };

        IQueryable<StudentEntity> studentQuery = _context.Students.Where(s => s.IsActive);

        if (classId.HasValue && classId.Value > 0)
            studentQuery = studentQuery.Where(s => s.ClassId == classId.Value);

        var students = await studentQuery
            .OrderBy(s => s.FullName)
            .Select(s => new { s.Id, s.FullName, s.RollNumber, ClassName = s.Class != null ? s.Class.ClassName : null })
            .ToListAsync();

        var studentIds = students.Select(s => s.Id).ToList();

        if (studentIds.Count == 0)
            return viewModel;

        IQueryable<StudentAttendanceEntity> attendanceQuery = _context.Attendances
            .Where(a => studentIds.Contains(a.StudentId));

        if (fromDate.HasValue)
            attendanceQuery = attendanceQuery.Where(a => a.AttendanceDate >= fromDate.Value.Date);

        if (toDate.HasValue)
            attendanceQuery = attendanceQuery.Where(a => a.AttendanceDate <= toDate.Value.Date);

        var rawAttendance = await attendanceQuery
            .Select(a => new { a.StudentId, a.Status })
            .ToListAsync();

        var attendanceLookup = rawAttendance
            .GroupBy(a => a.StudentId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    Present = g.Count(a => a.Status == "Present"),
                    Absent = g.Count(a => a.Status == "Absent"),
                    Leave = g.Count(a => a.Status == "Leave")
                }
            );

        foreach (var student in students)
        {
            attendanceLookup.TryGetValue(student.Id, out var stats);
            viewModel.Rows.Add(new AttendanceReportRow
            {
                StudentId = student.Id,
                StudentName = student.FullName,
                RollNumber = student.RollNumber,
                ClassName = student.ClassName ?? string.Empty,
                TotalPresent = stats?.Present ?? 0,
                TotalAbsent = stats?.Absent ?? 0,
                TotalLeave = stats?.Leave ?? 0
            });
        }

        return viewModel;
    }

    public async Task<DailyAttendanceViewModel> CopyFromPreviousDayAsync(DateTime date, int classId)
    {
        date = ToUtc(date);
        var previousDate = date.AddDays(-1);
        var previousAttendance = await GetAttendanceSheetAsync(previousDate, classId);
        var currentSheet = await GetAttendanceSheetAsync(date, classId);
        
        foreach (var currentStudent in currentSheet.Students)
        {
            var previousStudent = previousAttendance.Students
                .FirstOrDefault(s => s.StudentId == currentStudent.StudentId);
            
            if (previousStudent != null)
            {
                currentStudent.Status = previousStudent.Status;
                currentStudent.Remarks = previousStudent.Remarks;
            }
        }
        
        return currentSheet;
    }
}