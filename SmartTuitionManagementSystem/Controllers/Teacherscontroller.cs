using Microsoft.AspNetCore.Mvc;
using SmartTuitionManagementSystem.Models;
using SmartTuitionManagementSystem.Services;

namespace SmartTuitionManagementSystem.Controllers;

public class TeachersController : Controller
{
    private readonly ILogger<TeachersController> _logger;
    private readonly ITeacherService _teacherService;

    public TeachersController(ILogger<TeachersController> logger, ITeacherService teacherService)
    {
        _logger = logger;
        _teacherService = teacherService;
    }

    // GET: Teachers/ (Default route)
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return await TeacherList();
    }

    // GET: Teachers/TeacherList
    [HttpGet]
    public async Task<IActionResult> TeacherList()
    {
        if (HttpContext.Session.GetString("UserId") == null)
        {
            TempData["ErrorMessage"] = "Please login to access this page";
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var teachers = await _teacherService.GetAllTeachersAsync();
            return View(teachers ?? new List<TeacherViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting teacher list");
            TempData["ErrorMessage"] = "Error loading teachers. Please try again.";
            return View(new List<TeacherViewModel>());
        }
    }

    // GET: Teachers/AddTeacher
    [HttpGet]
    public IActionResult AddTeacher()
    {
        if (HttpContext.Session.GetString("UserId") == null)
        {
            TempData["ErrorMessage"] = "Please login to access this page";
            return RedirectToAction("Login", "Account");
        }
        
        return View();
    }

    // POST: Teachers/AddTeacher
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTeacher(TeacherViewModel model)
    {
        if (HttpContext.Session.GetString("UserId") == null)
        {
            TempData["ErrorMessage"] = "Please login to access this page";
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await _teacherService.AddTeacherAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("TeacherList");
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding teacher");
            ModelState.AddModelError("", "An error occurred while adding the teacher");
            return View(model);
        }
    }

    // GET: Teachers/EditTeacher/5
    [HttpGet]
    public async Task<IActionResult> EditTeacher(int id)
    {
        if (HttpContext.Session.GetString("UserId") == null)
        {
            TempData["ErrorMessage"] = "Please login to access this page";
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
            {
                TempData["ErrorMessage"] = "Teacher not found";
                return RedirectToAction("TeacherList");
            }

            return View(teacher);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting teacher for edit");
            TempData["ErrorMessage"] = "Error loading teacher details";
            return RedirectToAction("TeacherList");
        }
    }

    // POST: Teachers/EditTeacher/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTeacher(int id, TeacherViewModel model)
    {
        if (HttpContext.Session.GetString("UserId") == null)
        {
            TempData["ErrorMessage"] = "Please login to access this page";
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await _teacherService.UpdateTeacherAsync(id, model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("TeacherList");
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating teacher");
            ModelState.AddModelError("", "An error occurred while updating the teacher");
            return View(model);
        }
    }

    // POST: Teachers/DeleteTeacher/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTeacher(int id)
    {
        if (HttpContext.Session.GetString("UserId") == null)
        {
            return Json(new { success = false, message = "Please login" });
        }

        try
        {
            var result = await _teacherService.DeleteTeacherAsync(id);
            return Json(new { success = result.Success, message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting teacher");
            return Json(new { success = false, message = "An error occurred while deleting" });
        }
    }
}