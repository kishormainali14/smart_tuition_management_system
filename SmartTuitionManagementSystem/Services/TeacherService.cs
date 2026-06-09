using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Data;
using SmartTuitionManagementSystem.Entities;
using SmartTuitionManagementSystem.Models;

namespace SmartTuitionManagementSystem.Services;

public class TeacherService : ITeacherService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TeacherService> _logger;

    public TeacherService(ApplicationDbContext context, ILogger<TeacherService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(bool Success, string Message, int TeacherId)> AddTeacherAsync(TeacherViewModel model)
    {
        try
        {
            if (await IsEmailExistsAsync(model.Email))
            {
                return (false, "A teacher with this email already exists", 0);
            }

            var teacher = new TeacherEntity
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim().ToLower(),
                PhoneNumber = model.PhoneNumber.Trim(),
                Address = model.Address?.Trim() ?? string.Empty,
                Qualification = model.Qualification.Trim(),
                ExperienceYears = model.ExperienceYears,
                Specialization = model.Specialization?.Trim() ?? string.Empty,
                HireDate = DateTime.SpecifyKind(model.HireDate, DateTimeKind.Utc),
                Salary = model.Salary,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Teacher added successfully: {Email}", teacher.Email);
            return (true, "Teacher added successfully!", teacher.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding teacher");
            return (false, $"Error: {ex.Message}", 0);
        }
    }

    public async Task<List<TeacherViewModel>> GetAllTeachersAsync()
    {
        var teachers = await _context.Teachers
            .Where(t => t.IsActive)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TeacherViewModel
            {
                Id = t.Id,
                FullName = t.FullName,
                Email = t.Email,
                PhoneNumber = t.PhoneNumber,
                Address = t.Address,
                Qualification = t.Qualification,
                ExperienceYears = t.ExperienceYears,
                Specialization = t.Specialization,
                HireDate = t.HireDate,
                Salary = t.Salary
            })
            .ToListAsync();

        return teachers;
    }

    public async Task<TeacherViewModel?> GetTeacherByIdAsync(int id)
    {
        var teacher = await _context.Teachers
            .Where(t => t.Id == id && t.IsActive)
            .Select(t => new TeacherViewModel
            {
                Id = t.Id,
                FullName = t.FullName,
                Email = t.Email,
                PhoneNumber = t.PhoneNumber,
                Address = t.Address,
                Qualification = t.Qualification,
                ExperienceYears = t.ExperienceYears,
                Specialization = t.Specialization,
                HireDate = t.HireDate,
                Salary = t.Salary
            })
            .FirstOrDefaultAsync();

        return teacher;
    }

    public async Task<(bool Success, string Message)> UpdateTeacherAsync(int id, TeacherViewModel model)
    {
        try
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return (false, "Teacher not found");
            }

            if (await IsEmailExistsAsync(model.Email, id))
            {
                return (false, "Another teacher with this email already exists");
            }

            teacher.FullName = model.FullName.Trim();
            teacher.Email = model.Email.Trim().ToLower();
            teacher.PhoneNumber = model.PhoneNumber.Trim();
            teacher.Address = model.Address?.Trim() ?? string.Empty;
            teacher.Qualification = model.Qualification.Trim();
            teacher.ExperienceYears = model.ExperienceYears;
            teacher.Specialization = model.Specialization?.Trim() ?? string.Empty;
            teacher.HireDate = DateTime.SpecifyKind(model.HireDate, DateTimeKind.Utc);
            teacher.Salary = model.Salary;
            teacher.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Teacher updated successfully: {Id}", id);
            return (true, "Teacher updated successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating teacher: {Id}", id);
            return (false, $"Error: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> DeleteTeacherAsync(int id)
    {
        try
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return (false, "Teacher not found");
            }

            teacher.IsActive = false;
            teacher.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Teacher deleted successfully: {Id}", id);
            return (true, "Teacher deleted successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting teacher: {Id}", id);
            return (false, $"Error: {ex.Message}");
        }
    }

    public async Task<bool> IsEmailExistsAsync(string email, int? excludeId = null)
    {
        var normalizedEmail = email.Trim().ToLower();
        
        if (excludeId.HasValue)
        {
            return await _context.Teachers.AnyAsync(t => 
                t.Email == normalizedEmail && t.Id != excludeId && t.IsActive);
        }
        
        return await _context.Teachers.AnyAsync(t => 
            t.Email == normalizedEmail && t.IsActive);
    }
}