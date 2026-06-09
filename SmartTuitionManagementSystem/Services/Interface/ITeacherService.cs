using SmartTuitionManagementSystem.Models;

namespace SmartTuitionManagementSystem.Services;

public interface ITeacherService
{
    Task<(bool Success, string Message, int TeacherId)> AddTeacherAsync(TeacherViewModel model);
    Task<List<TeacherViewModel>> GetAllTeachersAsync();
    Task<TeacherViewModel?> GetTeacherByIdAsync(int id);
    Task<(bool Success, string Message)> UpdateTeacherAsync(int id, TeacherViewModel model);
    Task<(bool Success, string Message)> DeleteTeacherAsync(int id);
    Task<bool> IsEmailExistsAsync(string email, int? excludeId = null);
}