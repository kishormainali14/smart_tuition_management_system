using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Data;
using SmartTuitionManagementSystem.Entities;
using SmartTuitionManagementSystem.Models;

namespace SmartTuitionManagementSystem.Services;

public class ClassService : IClassService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ClassService> _logger;

    public ClassService(ApplicationDbContext context, ILogger<ClassService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Get all classes - FIXED
    public async Task<List<ClassViewModel>> GetAllClassesAsync()
    {
        try
        {
            var classes = await _context.Classes
                .Where(c => c.IsActive)
                .OrderBy(c => c.ClassName)
                .Select(c => new ClassViewModel
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    ClassCode = c.ClassCode,
                    Section = c.Section,
                    MaxCapacity = c.MaxCapacity,
                    CurrentStrength = c.CurrentStrength,
                    RoomNumber = c.RoomNumber,
                    Description = c.Description,
                    AcademicYear = c.AcademicYear,
                    IsActive = c.IsActive
                    // Removed FullClassName from here - it's not in database
                })
                .AsNoTracking()
                .ToListAsync();

            // Set FullClassName after retrieving from database
            foreach (var classItem in classes)
            {
                classItem.FullClassName = $"{classItem.ClassName} {(string.IsNullOrEmpty(classItem.Section) ? "" : "- " + classItem.Section)}".Trim();
            }

            return classes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all classes");
            return new List<ClassViewModel>();
        }
    }

    // Get class by ID - FIXED
    public async Task<ClassViewModel?> GetClassByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
                return null;

            var classEntity = await _context.Classes
                .Where(c => c.Id == id && c.IsActive)
                .Select(c => new ClassViewModel
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    ClassCode = c.ClassCode,
                    Section = c.Section,
                    MaxCapacity = c.MaxCapacity,
                    CurrentStrength = c.CurrentStrength,
                    RoomNumber = c.RoomNumber,
                    Description = c.Description,
                    AcademicYear = c.AcademicYear,
                    IsActive = c.IsActive
                })
                .FirstOrDefaultAsync();

            if (classEntity != null)
            {
                // Set FullClassName after retrieving from database
                classEntity.FullClassName = $"{classEntity.ClassName} {(string.IsNullOrEmpty(classEntity.Section) ? "" : "- " + classEntity.Section)}".Trim();
            }

            return classEntity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting class by ID: {Id}", id);
            return null;
        }
    }

    // Create new class
    public async Task<(bool Success, string Message)> CreateClassAsync(ClassViewModel model)
    {
        try
        {
            _logger.LogInformation("Creating new class: {ClassName}", model.ClassName);

            if (string.IsNullOrWhiteSpace(model.ClassName))
                return (false, "Class name is required");

            // Check if class already exists
            var existingClass = await _context.Classes
                .FirstOrDefaultAsync(c => c.ClassName == model.ClassName && c.IsActive);
            
            if (existingClass != null)
                return (false, $"Class '{model.ClassName}' already exists");

            var classEntity = new ClassEntity
            {
                ClassName = model.ClassName.Trim(),
                ClassCode = model.ClassCode?.Trim() ?? GenerateClassCode(model.ClassName),
                Section = model.Section?.Trim() ?? "A",
                MaxCapacity = model.MaxCapacity > 0 ? model.MaxCapacity : 40,
                CurrentStrength = 0,
                RoomNumber = model.RoomNumber?.Trim() ?? string.Empty,
                Description = model.Description?.Trim() ?? string.Empty,
                AcademicYear = model.AcademicYear ?? DateTime.Now.Year.ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Classes.AddAsync(classEntity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Class created successfully: {ClassName}", classEntity.ClassName);
            return (true, "Class created successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating class: {ClassName}", model.ClassName);
            return (false, $"An error occurred: {ex.Message}");
        }
    }

    // Update class
    public async Task<(bool Success, string Message)> UpdateClassAsync(int id, ClassViewModel model)
    {
        try
        {
            _logger.LogInformation("Updating class with ID: {Id}", id);

            if (id <= 0)
                return (false, "Invalid class ID");

            var classEntity = await _context.Classes.FindAsync(id);
            
            if (classEntity == null || !classEntity.IsActive)
                return (false, "Class not found");

            // Check if another class with same name exists
            var existingClass = await _context.Classes
                .FirstOrDefaultAsync(c => c.ClassName == model.ClassName && c.Id != id && c.IsActive);
            
            if (existingClass != null)
                return (false, $"Another class with name '{model.ClassName}' already exists");

            // Update properties
            classEntity.ClassName = model.ClassName.Trim();
            classEntity.ClassCode = model.ClassCode?.Trim() ?? GenerateClassCode(model.ClassName);
            classEntity.Section = model.Section?.Trim() ?? "A";
            classEntity.MaxCapacity = model.MaxCapacity > 0 ? model.MaxCapacity : 40;
            classEntity.RoomNumber = model.RoomNumber?.Trim() ?? string.Empty;
            classEntity.Description = model.Description?.Trim() ?? string.Empty;
            classEntity.AcademicYear = model.AcademicYear ?? DateTime.Now.Year.ToString();
            classEntity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Class updated successfully: {ClassName}", classEntity.ClassName);
            return (true, "Class updated successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating class: {Id}", id);
            return (false, $"An error occurred: {ex.Message}");
        }
    }

    // Delete class (soft delete)
    public async Task<(bool Success, string Message)> DeleteClassAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting class with ID: {Id}", id);

            if (id <= 0)
                return (false, "Invalid class ID");

            var classEntity = await _context.Classes.FindAsync(id);
            
            if (classEntity == null)
                return (false, "Class not found");

            // Check if class has students
            var hasStudents = await _context.Students.AnyAsync(s => s.ClassId == id && s.IsActive);
            if (hasStudents)
                return (false, "Cannot delete class with enrolled students. Please reassign students first.");

            classEntity.IsActive = false;
            classEntity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Class deleted successfully: {ClassName}", classEntity.ClassName);
            return (true, "Class deleted successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting class: {Id}", id);
            return (false, $"An error occurred: {ex.Message}");
        }
    }

    // Helper method to generate class code
    private string GenerateClassCode(string className)
    {
        if (className.Contains("Class", StringComparison.OrdinalIgnoreCase))
        {
            var number = System.Text.RegularExpressions.Regex.Match(className, @"\d+");
            if (number.Success)
                return $"C{number.Value}";
        }
        
        return className.Replace(" ", "").ToUpper()[..Math.Min(5, className.Length)];
    }

    // ==================== SIMPLIFIED INTERFACE METHODS ====================
    
    public Task<List<TeacherViewModel>> GetAllTeachersAsync()
    {
        return Task.FromResult(new List<TeacherViewModel>());
    }

    public Task<(bool Success, string Message)> EnrollStudentInClassAsync(int classId, int studentId)
    {
        return Task.FromResult((true, "Not implemented"));
    }

    public Task<(bool Success, string Message)> RemoveStudentFromClassAsync(int classId, int studentId)
    {
        return Task.FromResult((true, "Not implemented"));
    }

    public Task<List<ClassStudentViewModel>> GetClassStudentsAsync(int classId)
    {
        return Task.FromResult(new List<ClassStudentViewModel>());
    }

    public Task<List<StudentSimpleViewModel>> GetAvailableStudentsAsync(int classId)
    {
        return Task.FromResult(new List<StudentSimpleViewModel>());
    }

    public Task<ClassStatisticsViewModel> GetClassStatisticsAsync(int classId)
    {
        return Task.FromResult(new ClassStatisticsViewModel());
    }
}