using SmartTuitionManagementSystem.Models;

namespace SmartTuitionManagementSystem.Services;

public interface IClassService
{
    // Class CRUD
    Task<List<ClassViewModel>> GetAllClassesAsync();
    Task<ClassViewModel?> GetClassByIdAsync(int id);
    Task<(bool Success, string Message)> CreateClassAsync(ClassViewModel model);
    Task<(bool Success, string Message)> UpdateClassAsync(int id, ClassViewModel model);
    Task<(bool Success, string Message)> DeleteClassAsync(int id);
    
    // These are optional - return empty for now
    Task<List<TeacherViewModel>> GetAllTeachersAsync();
    Task<(bool Success, string Message)> EnrollStudentInClassAsync(int classId, int studentId);
    Task<(bool Success, string Message)> RemoveStudentFromClassAsync(int classId, int studentId);
    Task<List<ClassStudentViewModel>> GetClassStudentsAsync(int classId);
    Task<List<StudentSimpleViewModel>> GetAvailableStudentsAsync(int classId);
    Task<ClassStatisticsViewModel> GetClassStatisticsAsync(int classId);
}