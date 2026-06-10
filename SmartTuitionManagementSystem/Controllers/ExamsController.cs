using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTuitionManagementSystem.Entities;
using SmartTuitionManagementSystem.Models;
using SmartTuitionManagementSystem.Services.Interfaces;

namespace SmartTuitionManagementSystem.Controllers;

public class ExamsController : Controller
{
    private readonly IExamService _examService;

    public ExamsController(IExamService examService)
    {
        _examService = examService;
    }

    private bool IsUserAuthenticated()
    {
        return HttpContext.Session.GetString("UserId") != null;
    }

    private IActionResult RedirectToLogin()
    {
        TempData["ErrorMessage"] = "Please login to access this page";
        return RedirectToAction("Login", "Account");
    }

    // GET: Exams
    public async Task<IActionResult> Index(string searchTerm, string examType, string status, int page = 1)
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        int pageSize = 10;
        var exams = await _examService.GetFilteredExamsAsync(searchTerm, examType, status);
        
        var totalCount = exams.Count;
        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ViewBag.CurrentPage = page;
        ViewBag.SearchTerm = searchTerm;
        ViewBag.SelectedExamType = examType;
        ViewBag.SelectedStatus = status;
        
        ViewBag.ExamTypeList = await _examService.GetExamTypeOptionsAsync();
        ViewBag.StatusList = new List<SelectListItem>
        {
            new SelectListItem { Value = "All", Text = "All" },
            new SelectListItem { Value = "Active", Text = "Active" },
            new SelectListItem { Value = "Inactive", Text = "Inactive" }
        };

        var pagedExams = exams.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return View("Index", pagedExams);
    }

    // GET: Exams/Create
    public async Task<IActionResult> Create()
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        var model = new ExamViewModel
        {
            ExamCode = await _examService.GenerateUniqueExamCodeAsync(),
            ExamTypeOptions = await _examService.GetExamTypeOptionsAsync(),
            AcademicYearOptions = await _examService.GetAcademicYearOptionsAsync(),
            SectionOptions = await _examService.GetSectionOptionsAsync(),
            IsActive = true
        };

        return View("Create", model);
    }

    // POST: Exams/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExamViewModel model)
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        if (!ModelState.IsValid)
        {
            model.ExamTypeOptions = await _examService.GetExamTypeOptionsAsync();
            model.AcademicYearOptions = await _examService.GetAcademicYearOptionsAsync();
            model.SectionOptions = await _examService.GetSectionOptionsAsync();
            return View("Create", model);
        }

        var result = await _examService.CreateExamAsync(model);

        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", result.Message);
        model.ExamTypeOptions = await _examService.GetExamTypeOptionsAsync();
        model.AcademicYearOptions = await _examService.GetAcademicYearOptionsAsync();
        model.SectionOptions = await _examService.GetSectionOptionsAsync();
        return View("Create", model);
    }

    // GET: Exams/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        var exam = await _examService.GetExamByIdAsync(id);
        if (exam == null)
        {
            TempData["ErrorMessage"] = "Exam not found";
            return RedirectToAction(nameof(Index));
        }

        var model = new ExamViewModel
        {
            Id = exam.Id,
            ExamName = exam.ExamName,
            ExamCode = exam.ExamCode,
            ExamType = exam.ExamType,
            AcademicYear = exam.AcademicYear,
            Section = exam.Section,
            Description = exam.Description,
            IsActive = exam.IsActive,
            CreatedAt = exam.CreatedAt,
            UpdatedAt = exam.UpdatedAt,
            ExamTypeOptions = await _examService.GetExamTypeOptionsAsync(),
            AcademicYearOptions = await _examService.GetAcademicYearOptionsAsync(),
            SectionOptions  = await _examService.GetSectionOptionsAsync()
        };

        return View("Edit", model);
    }

    // POST: Exams/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExamViewModel model)
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        if (id != model.Id)
        {
            TempData["ErrorMessage"] = "Exam ID mismatch";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            model.ExamTypeOptions = await _examService.GetExamTypeOptionsAsync();
            model.AcademicYearOptions = await _examService.GetAcademicYearOptionsAsync();
            model.SectionOptions = await _examService.GetSectionOptionsAsync();
            return View("Edit", model);
        }

        var result = await _examService.UpdateExamAsync(id, model);

        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", result.Message);
        model.ExamTypeOptions = await _examService.GetExamTypeOptionsAsync();
        model.AcademicYearOptions = await _examService.GetAcademicYearOptionsAsync();
        model.SectionOptions = await _examService.GetSectionOptionsAsync();
        return View("Edit", model);
    }

    // GET: Exams/Details/5
    public async Task<IActionResult> Details(int id)
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        var exam = await _examService.GetExamByIdAsync(id);
        if (exam == null)
        {
            TempData["ErrorMessage"] = "Exam not found";
            return RedirectToAction(nameof(Index));
        }

        return View("Details", exam);
    }

    // POST: Exams/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsUserAuthenticated())
        {
            return Json(new { success = false, message = "Please login to perform this action" });
        }

        var result = await _examService.DeleteExamAsync(id);
        return Json(new { success = result.Success, message = result.Message });
    }

    // POST: Exams/ToggleStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        if (!IsUserAuthenticated())
        {
            return Json(new { success = false, message = "Please login to perform this action" });
        }

        var result = await _examService.ToggleExamStatusAsync(id);
        return Json(new { success = result.Success, message = result.Message });
    }

    // GET: Exams/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        var dashboard = await _examService.GetExamDashboardStatsAsync();
        return View("Dashboard", dashboard);
    }
}