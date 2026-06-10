using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTuitionManagementSystem.Entities;
using SmartTuitionManagementSystem.Models;

namespace SmartTuitionManagementSystem.Services.Interfaces;

public interface IExamService
{
    // Basic CRUD Operations
    Task<List<ExamEntity>> GetAllExamsAsync();
    Task<ExamEntity?> GetExamByIdAsync(int id);
    Task<(bool Success, string Message)> CreateExamAsync(ExamViewModel model);
    Task<(bool Success, string Message)> UpdateExamAsync(int id, ExamViewModel model);
    Task<(bool Success, string Message)> DeleteExamAsync(int id);
    
    // Additional Features
    Task<(bool Success, string Message)> ToggleExamStatusAsync(int id);
    Task<bool> IsExamCodeExistsAsync(string examCode, int? excludeId = null);
    Task<string> GenerateUniqueExamCodeAsync();
    Task<ExamDashboardViewModel> GetExamDashboardStatsAsync();
    
    // Filter and Search
    Task<List<ExamEntity>> GetFilteredExamsAsync(string? searchTerm, string? examType, string? status);
    
    // Dropdown Options
    Task<IEnumerable<SelectListItem>> GetExamTypeOptionsAsync();
    Task<IEnumerable<SelectListItem>> GetAcademicYearOptionsAsync();
    Task<IEnumerable<SelectListItem>> GetSectionOptionsAsync();  // Changed from GetSemesterOptionsAsync
}