using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Data;
using SmartTuitionManagementSystem.Entities;
using SmartTuitionManagementSystem.Models;

namespace SmartTuitionManagementSystem.Services;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StudentService> _logger;

    public StudentService(ApplicationDbContext context, ILogger<StudentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Helper method to convert DateTime to UTC
    private static DateTime? ToUtc(DateTime? date)
    {
        if (!date.HasValue) return null;
        return date.Value.Kind == DateTimeKind.Utc ? date.Value : DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
    }

    private static DateTime ToUtc(DateTime date)
    {
        return date.Kind == DateTimeKind.Utc ? date : DateTime.SpecifyKind(date, DateTimeKind.Utc);
    }

    // Add new student
    public async Task<(bool Success, string Message, int StudentId)> AddStudentAsync(StudentViewModel model)
    {
        try
        {
            _logger.LogInformation("Attempting to add new student with email: {Email}", model.Email);

            // Validate required fields
            if (string.IsNullOrWhiteSpace(model.FullName))
                return (false, "Full name is required", 0);
            
            if (string.IsNullOrWhiteSpace(model.Email))
                return (false, "Email is required", 0);

            // Check if email already exists
            if (await IsEmailExistsAsync(model.Email))
            {
                _logger.LogWarning("Email already exists: {Email}", model.Email);
                return (false, "A student with this email already exists", 0);
            }

            var now = DateTime.UtcNow;

            // Create new student entity with UTC dates
            var student = new StudentEntity
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim().ToLower(),
                PhoneNumber = model.PhoneNumber?.Trim() ?? string.Empty,
                Address = model.Address?.Trim() ?? string.Empty,
                DateOfBirth = ToUtc(model.DateOfBirth),
                Grade = model.Grade?.Trim() ?? string.Empty,
                ParentName = model.ParentName?.Trim() ?? string.Empty,
                ParentPhone = model.ParentPhone?.Trim() ?? string.Empty,
                EnrollmentDate = now,
                ClassId = 1, // TEMPORARY: Set default ClassId until dynamic dropdown is implemented
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = null
            };

            // Save to database
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Student added successfully: ID: {StudentId}, Email: {Email}, Name: {FullName}", 
                student.Id, student.Email, student.FullName);
            
            return (true, "Student added successfully!", student.Id);
        }
        catch (DbUpdateException dbEx)
        {
            var innerMsg = dbEx.InnerException?.Message ?? dbEx.Message;
            _logger.LogError(dbEx, "Database error while adding student: {Email}. Error: {Error}", model.Email, innerMsg);
            return (false, $"Database error: {innerMsg}", 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding student: {Email}", model.Email);
            return (false, $"An error occurred: {ex.Message}", 0);
        }
    }

    // Get all students
    public async Task<List<StudentViewModel>> GetAllStudentsAsync()
    {
        try
        {
            var students = await _context.Students
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new StudentViewModel
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    Address = s.Address,
                    DateOfBirth = s.DateOfBirth,
                    Grade = s.Grade,
                    ParentName = s.ParentName,
                    ParentPhone = s.ParentPhone,
                    EnrollmentDate = s.EnrollmentDate,
                    IsActive = s.IsActive
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} students", students.Count);
            return students;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all students");
            return new List<StudentViewModel>();
        }
    }

    // Get student by ID
    public async Task<StudentViewModel?> GetStudentByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid student ID requested: {Id}", id);
                return null;
            }

            var student = await _context.Students
                .Where(s => s.Id == id && s.IsActive)
                .Select(s => new StudentViewModel
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    Address = s.Address,
                    DateOfBirth = s.DateOfBirth,
                    Grade = s.Grade,
                    ParentName = s.ParentName,
                    ParentPhone = s.ParentPhone,
                    EnrollmentDate = s.EnrollmentDate,
                    IsActive = s.IsActive
                })
                .FirstOrDefaultAsync();

            if (student == null)
            {
                _logger.LogWarning("Student not found with ID: {Id}", id);
            }

            return student;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student by ID: {Id}", id);
            return null;
        }
    }

    // Update student
    public async Task<(bool Success, string Message)> UpdateStudentAsync(int id, StudentViewModel model)
    {
        try
        {
            _logger.LogInformation("Attempting to update student with ID: {Id}", id);

            if (id <= 0)
                return (false, "Invalid student ID");

            var student = await _context.Students.FindAsync(id);
            
            if (student == null || !student.IsActive)
            {
                _logger.LogWarning("Student not found or inactive for update. ID: {Id}", id);
                return (false, "Student not found or has been deleted");
            }

            // Check if email exists for another student
            if (await IsEmailExistsAsync(model.Email, id))
            {
                _logger.LogWarning("Email already exists for another student. ID: {Id}, Email: {Email}", id, model.Email);
                return (false, "Another student with this email already exists");
            }

            // Update student details
            student.FullName = model.FullName.Trim();
            student.Email = model.Email.Trim().ToLower();
            student.PhoneNumber = model.PhoneNumber?.Trim() ?? string.Empty;
            student.Address = model.Address?.Trim() ?? string.Empty;
            student.DateOfBirth = ToUtc(model.DateOfBirth);
            student.Grade = model.Grade?.Trim() ?? string.Empty;
            student.ParentName = model.ParentName?.Trim() ?? string.Empty;
            student.ParentPhone = model.ParentPhone?.Trim() ?? string.Empty;
            student.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Student updated successfully. ID: {Id}, Email: {Email}, Name: {FullName}", 
                id, student.Email, student.FullName);
            
            return (true, "Student updated successfully!");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency conflict while updating student. ID: {Id}", id);
            return (false, "The student record was modified by another user. Please refresh and try again.");
        }
        catch (DbUpdateException dbEx)
        {
            var innerMsg = dbEx.InnerException?.Message ?? dbEx.Message;
            _logger.LogError(dbEx, "Database error while updating student: {Id}. Error: {Error}", id, innerMsg);
            return (false, $"Database error: {innerMsg}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating student: {Id}", id);
            return (false, $"An error occurred: {ex.Message}");
        }
    }

    // Delete student (soft delete)
    public async Task<(bool Success, string Message)> DeleteStudentAsync(int id)
    {
        try
        {
            _logger.LogInformation("Attempting to delete student with ID: {Id}", id);

            if (id <= 0)
                return (false, "Invalid student ID");

            var student = await _context.Students.FindAsync(id);
            
            if (student == null)
            {
                _logger.LogWarning("Student not found for deletion. ID: {Id}", id);
                return (false, "Student not found");
            }

            if (!student.IsActive)
            {
                _logger.LogWarning("Student already inactive. ID: {Id}", id);
                return (false, "Student is already deleted");
            }

            student.IsActive = false;
            student.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Student deleted successfully. ID: {Id}, Email: {Email}", id, student.Email);
            return (true, "Student deleted successfully!");
        }
        catch (DbUpdateException dbEx)
        {
            var innerMsg = dbEx.InnerException?.Message ?? dbEx.Message;
            _logger.LogError(dbEx, "Database error while deleting student: {Id}. Error: {Error}", id, innerMsg);
            return (false, $"Database error: {innerMsg}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting student: {Id}", id);
            return (false, $"An error occurred: {ex.Message}");
        }
    }

    // Check if email exists
    public async Task<bool> IsEmailExistsAsync(string email, int? excludeId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var normalizedEmail = email.Trim().ToLower();
            
            if (excludeId.HasValue)
            {
                return await _context.Students.AnyAsync(s => 
                    s.Email == normalizedEmail && 
                    s.Id != excludeId.Value && 
                    s.IsActive);
            }
            
            return await _context.Students.AnyAsync(s => 
                s.Email == normalizedEmail && 
                s.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email existence: {Email}", email);
            return false;
        }
    }
}