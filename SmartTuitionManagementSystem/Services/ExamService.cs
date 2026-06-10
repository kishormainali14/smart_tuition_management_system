using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Data;
using SmartTuitionManagementSystem.Entities;
using SmartTuitionManagementSystem.Models;
using SmartTuitionManagementSystem.Services.Interfaces;
using System.Text.RegularExpressions;

namespace SmartTuitionManagementSystem.Services;

public class ExamService : IExamService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExamService> _logger;

    public ExamService(ApplicationDbContext context, ILogger<ExamService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ExamEntity>> GetAllExamsAsync()
    {
        try
        {
            return await _context.Exams
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all exams");
            return new List<ExamEntity>();
        }
    }

    public async Task<ExamEntity?> GetExamByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
                return null;

            return await _context.Exams
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting exam by ID: {Id}", id);
            return null;
        }
    }

    public async Task<(bool Success, string Message)> CreateExamAsync(ExamViewModel model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.ExamName))
                return (false, "Exam name is required");

            if (await IsExamCodeExistsAsync(model.ExamCode))
            {
                model.ExamCode = await GenerateUniqueExamCodeAsync();
                return (false, "Exam code already exists. New code generated: " + model.ExamCode);
            }

            var exam = new ExamEntity
            {
                ExamName = model.ExamName.Trim(),
                ExamCode = model.ExamCode,
                ExamType = model.ExamType,
                AcademicYear = model.AcademicYear,
                Section = model.Section,
                Description = model.Description?.Trim() ?? string.Empty,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Exams.AddAsync(exam);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Exam created successfully: {ExamName} ({ExamCode})", exam.ExamName, exam.ExamCode);
            return (true, $"Exam '{exam.ExamName}' created successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating exam: {ExamName}", model.ExamName);
            return (false, $"An error occurred: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> UpdateExamAsync(int id, ExamViewModel model)
    {
        try
        {
            if (id <= 0)
                return (false, "Invalid exam ID");

            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
                return (false, "Exam not found");

            if (await IsExamCodeExistsAsync(model.ExamCode, id))
                return (false, "Exam code already exists. Please use a different code.");

            exam.ExamName = model.ExamName.Trim();
            exam.ExamCode = model.ExamCode;
            exam.ExamType = model.ExamType;
            exam.AcademicYear = model.AcademicYear;
            exam.Section = model.Section;
            exam.Description = model.Description?.Trim() ?? string.Empty;
            exam.IsActive = model.IsActive;
            exam.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Exam updated successfully: {ExamName} (ID: {Id})", exam.ExamName, id);
            return (true, $"Exam '{exam.ExamName}' updated successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating exam: {Id}", id);
            return (false, $"An error occurred: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> DeleteExamAsync(int id)
    {
        try
        {
            if (id <= 0)
                return (false, "Invalid exam ID");

            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
                return (false, "Exam not found");

            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Exam deleted successfully: {ExamName} (ID: {Id})", exam.ExamName, id);
            return (true, "Exam deleted successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting exam: {Id}", id);
            return (false, $"An error occurred: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> ToggleExamStatusAsync(int id)
    {
        try
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
                return (false, "Exam not found");

            exam.IsActive = !exam.IsActive;
            exam.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var status = exam.IsActive ? "activated" : "deactivated";
            _logger.LogInformation("Exam {Status}: {ExamName} (ID: {Id})", status, exam.ExamName, id);
            return (true, $"Exam {status} successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling exam status: {Id}", id);
            return (false, $"An error occurred: {ex.Message}");
        }
    }

    public async Task<bool> IsExamCodeExistsAsync(string examCode, int? excludeId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(examCode))
                return false;

            var query = _context.Exams.Where(e => e.ExamCode == examCode);
            
            if (excludeId.HasValue)
                query = query.Where(e => e.Id != excludeId.Value);

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking exam code existence: {ExamCode}", examCode);
            return false;
        }
    }

    public async Task<string> GenerateUniqueExamCodeAsync()
    {
        try
        {
            var lastExam = await _context.Exams
                .OrderByDescending(e => e.Id)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            
            if (lastExam != null && !string.IsNullOrEmpty(lastExam.ExamCode))
            {
                var numericPart = Regex.Match(lastExam.ExamCode, @"\d+");
                if (numericPart.Success)
                {
                    nextNumber = int.Parse(numericPart.Value) + 1;
                }
            }
            
            var newCode = $"EXAM{nextNumber:D3}";
            
            while (await IsExamCodeExistsAsync(newCode))
            {
                nextNumber++;
                newCode = $"EXAM{nextNumber:D3}";
            }
            
            return newCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating unique exam code");
            return $"EXAM{Guid.NewGuid().ToString().Substring(0, 5)}";
        }
    }

    public async Task<ExamDashboardViewModel> GetExamDashboardStatsAsync()
    {
        try
        {
            var exams = await _context.Exams.ToListAsync();
            var today = DateTime.Today;
            
            return new ExamDashboardViewModel
            {
                TotalExams = exams.Count,
                ActiveExams = exams.Count(e => e.IsActive),
                InactiveExams = exams.Count(e => !e.IsActive),
                UpcomingExams = exams.Count(e => e.CreatedAt.Date > today && e.IsActive),
                OngoingExams = exams.Count(e => e.CreatedAt.Date == today && e.IsActive),
                CompletedExams = exams.Count(e => e.CreatedAt.Date < today)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting exam dashboard stats");
            return new ExamDashboardViewModel();
        }
    }

    public async Task<List<ExamEntity>> GetFilteredExamsAsync(string? searchTerm, string? examType, string? status)
    {
        try
        {
            var query = _context.Exams.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.ExamName.Contains(searchTerm) || e.ExamCode.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(examType) && examType != "All")
            {
                query = query.Where(e => e.ExamType == examType);
            }

            if (!string.IsNullOrEmpty(status))
            {
                bool isActive = status == "Active";
                query = query.Where(e => e.IsActive == isActive);
            }

            return await query
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting filtered exams");
            return new List<ExamEntity>();
        }
    }

    public async Task<IEnumerable<SelectListItem>> GetExamTypeOptionsAsync()
    {
        return await Task.Run(() => new List<SelectListItem>
        {
            new SelectListItem { Value = "Unit Test", Text = "Unit Test" },
            new SelectListItem { Value = "First Terminal", Text = "First Terminal" },
            new SelectListItem { Value = "Second Terminal", Text = "Second Terminal" },
            new SelectListItem { Value = "Mid-Term", Text = "Mid-Term" },
            new SelectListItem { Value = "Pre-Board", Text = "Pre-Board" },
            new SelectListItem { Value = "Final", Text = "Final Examination" },
            new SelectListItem { Value = "Practical", Text = "Practical" },
            new SelectListItem { Value = "Viva", Text = "Viva Voce" },
            new SelectListItem { Value = "Supplementary", Text = "Supplementary" },
            new SelectListItem { Value = "Quiz", Text = "Quiz" },
            new SelectListItem { Value = "Assignment", Text = "Assignment" }
        });
    }

    public async Task<IEnumerable<SelectListItem>> GetAcademicYearOptionsAsync()
    {
        return await Task.Run(() =>
        {
            var currentYear = DateTime.Now.Year;
            var years = new List<SelectListItem>();
            
            for (int i = currentYear - 3; i <= currentYear + 2; i++)
            {
                years.Add(new SelectListItem { Value = i.ToString(), Text = $"{i}-{i + 1}" });
            }
            
            return years.AsEnumerable();
        });
    }

    public async Task<IEnumerable<SelectListItem>> GetSectionOptionsAsync()
    {
        return await Task.Run(() => new List<SelectListItem>
        {
            new SelectListItem { Value = "Section A", Text = "Section A" },
            new SelectListItem { Value = "Section B", Text = "Section B" },
            new SelectListItem { Value = "Section C", Text = "Section C" },
            new SelectListItem { Value = "Section D", Text = "Section D" },
            new SelectListItem { Value = "Section E", Text = "Section E" }
        });
    }
}