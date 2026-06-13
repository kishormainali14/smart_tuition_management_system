using SmartTuitionManagementSystem.Models;

namespace SmartTuitionManagementSystem.Services;

public interface IStudentService
{
    // Basic CRUD operations
    Task<(bool Success, string Message, int StudentId)> AddStudentAsync(StudentViewModel model);
    Task<List<StudentViewModel>> GetAllStudentsAsync();
    Task<StudentViewModel?> GetStudentByIdAsync(int id);
    Task<(bool Success, string Message)> UpdateStudentAsync(int id, StudentViewModel model);
    Task<(bool Success, string Message)> DeleteStudentAsync(int id);
    Task<bool> IsEmailExistsAsync(string email, int? excludeId = null);
    Task<bool> IsRollNumberExistsAsync(string rollNumber, int classId, int? excludeId = null);
}