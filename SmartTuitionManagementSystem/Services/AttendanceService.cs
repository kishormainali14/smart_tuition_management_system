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
    
    public async Task<DailyAttendanceViewModel> GetAttendanceSheetAsync(string grade, DateTime date)
    {
        var viewModel = new DailyAttendanceViewModel
        {
            SelectedGrade = grade,
            SelectedDate = date,
            GradeList = await GetGradeListAsync()
        };
        
        // Get all students in the grade
        var students = await _context.Students
            .Where(s => s.Grade == grade && s.IsActive)
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
    
    public async Task<bool> SaveAttendanceAsync(DailyAttendanceViewModel model, int markedByUserId)
    {
        try
        {
            foreach (var studentRow in model.Students)
            {
                var existing = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.StudentId == studentRow.StudentId && 
                                              a.AttendanceDate.Date == model.SelectedDate.Date);
                
                if (existing != null)
                {
                    // Update existing
                    existing.Status = studentRow.Status;
                    existing.Remarks = studentRow.Remarks;
                    existing.MarkedAt = DateTime.UtcNow;
                    existing.MarkedBy = markedByUserId;
                }
                else
                {
                    // Create new
                    var attendance = new StudentAttendanceEntity
                    {
                        StudentId = studentRow.StudentId,
                        AttendanceDate = model.SelectedDate,
                        Status = studentRow.Status,
                        Remarks = studentRow.Remarks,
                        MarkedAt = DateTime.UtcNow,
                        MarkedBy = markedByUserId
                    };
                    await _context.Attendances.AddAsync(attendance);
                }
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving attendance: {ex.Message}");
            return false;
        }
    }
    
    public async Task<List<SelectListItem>> GetGradeListAsync()
    {
        // Get unique grades from students
        var grades = await _context.Students
            .Where(s => s.IsActive && !string.IsNullOrEmpty(s.Grade))
            .Select(s => s.Grade)
            .Distinct()
            .OrderBy(g => g)
            .ToListAsync();
        
        return grades.Select(g => new SelectListItem
        {
            Value = g,
            Text = g
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
        var lateDays = attendances.Count(a => a.Status == "Late");
        var excusedDays = attendances.Count(a => a.Status == "Excused");
        
        return new StudentAttendanceHistoryViewModel
        {
            StudentId = student.Id,
            StudentName = student.FullName,
            Grade = student.Grade,
            Email = student.Email,
            ParentPhone = student.ParentPhone,
            TotalDays = totalDays,
            PresentDays = presentDays,
            AbsentDays = absentDays,
            LateDays = lateDays,
            ExcusedDays = excusedDays,
            AttendancePercentage = totalDays > 0 ? (double)presentDays / totalDays * 100 : 0,
            Records = attendances.Select(a => new DailyAttendanceRecord
            {
                Date = a.AttendanceDate,
                Status = a.Status,
                Remarks = a.Remarks
            }).ToList()
        };
    }
    
    public async Task<bool> IsAttendanceMarkedAsync(string grade, DateTime date)
    {
        var studentIds = await _context.Students
            .Where(s => s.Grade == grade)
            .Select(s => s.Id)
            .ToListAsync();
        
        var count = await _context.Attendances
            .CountAsync(a => a.AttendanceDate.Date == date.Date && 
                            studentIds.Contains(a.StudentId));
        
        return count > 0;
    }
    
    public async Task<DailyAttendanceViewModel> CopyFromPreviousDayAsync(string grade, DateTime date)
    {
        var previousDate = date.AddDays(-1);
        var previousAttendance = await GetAttendanceSheetAsync(grade, previousDate);
        var currentSheet = await GetAttendanceSheetAsync(grade, date);
        
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