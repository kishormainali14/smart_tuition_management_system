using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTuitionManagementSystem.Models;
using SmartTuitionManagementSystem.Services;

namespace SmartTuitionManagementSystem.Controllers;

public class ClassController : Controller
{
    private readonly ILogger<ClassController> _logger;
    private readonly IClassService _classService;

    public ClassController(ILogger<ClassController> logger, IClassService classService)
    {
        _logger = logger;
        _classService = classService;
    }

    // Helper method to check authentication
    private bool IsUserAuthenticated()
    {
        return HttpContext.Session.GetString("UserId") != null;
    }

    // Helper method to redirect to login
    private IActionResult RedirectToLogin()
    {
        TempData["ErrorMessage"] = "Please login to access this page";
        return RedirectToAction("Login", "Account");
    }

    // Helper method to get grade/class options for dropdown
    private IEnumerable<SelectListItem> GetGradeClassOptions(string selectedValue = "")
    {
        var options = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "-- Select Grade --", Disabled = true }
        };
        
        var grades = new[] { "Nursery", "LKG", "UKG", "Class 1", "Class 2", "Class 3", "Class 4", "Class 5", 
                             "Class 6", "Class 7", "Class 8", "Class 9", "Class 10", "Class 11", "Class 12" };
        
        foreach (var grade in grades)
        {
            options.Add(new SelectListItem 
            { 
                Value = grade, 
                Text = grade,
                Selected = (selectedValue == grade)
            });
        }
        
        return options;
    }

    // GET: Class/Index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            if (!IsUserAuthenticated())
                return RedirectToLogin();

            var classes = await _classService.GetAllClassesAsync();
            return View("~/Views/Classes/Index.cshtml", classes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading classes list");
            TempData["ErrorMessage"] = "An error occurred while loading the classes";
            return View("~/Views/Classes/Index.cshtml", new List<ClassViewModel>());
        }
    }

    // GET: Class/AddClass
    [HttpGet]
    public IActionResult AddClass()
    {
        try
        {
            if (!IsUserAuthenticated())
                return RedirectToLogin();

            var model = new ClassViewModel
            {
                IsActive = true,
                MaxCapacity = 40,
                AcademicYear = DateTime.Now.Year.ToString(),
                GradeOptions = GetGradeClassOptions()
            };

            return View("~/Views/Classes/AddClass.cshtml", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading AddClass page");
            TempData["ErrorMessage"] = "An error occurred while loading the page";
            return RedirectToAction("Index");
        }
    }

    // POST: Class/AddClass
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddClass(ClassViewModel model)
    {
        try
        {
            if (!IsUserAuthenticated())
                return RedirectToLogin();

            // Remove GradeOptions from validation since it's for dropdown only
            ModelState.Remove("GradeOptions");
            ModelState.Remove("CurrentStrength");
            ModelState.Remove("AvailableSeats");
            ModelState.Remove("TotalStudents");
            ModelState.Remove("TotalSubjects");
            ModelState.Remove("TotalTeachers");
            ModelState.Remove("FullClassName");
            ModelState.Remove("TeacherOptions");
            ModelState.Remove("AcademicYearOptions");
            ModelState.Remove("SubjectAssignments");
            ModelState.Remove("Students");

            // Set default values
            model.CurrentStrength = 0;
            model.IsActive = true;

            // FIX: Check if ModelState is valid (missing ! operator was the issue)
            if (ModelState.IsValid)
            {
                var result = await _classService.CreateClassAsync(model);

                if (result.Success)
                {
                    _logger.LogInformation("Class added successfully. Grade: {Grade}, Section: {Section}", model.Grade, model.Section);
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning("Failed to add class: {Message}", result.Message);
                ModelState.AddModelError("", result.Message);
            }
            else
            {
                _logger.LogWarning("Model validation failed for class addition");
                // Log validation errors for debugging
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Validation error: {Error}", error.ErrorMessage);
                }
            }

            // Repopulate grade options with the selected value preserved
            model.GradeOptions = GetGradeClassOptions(model.ClassName);
            return View("~/Views/Classes/AddClass.cshtml", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding class");
            TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
            model.GradeOptions = GetGradeClassOptions(model.ClassName);
            return View("~/Views/Classes/AddClass.cshtml", model);
        }
    }

    // GET: Class/EditClass/5
    [HttpGet]
    public async Task<IActionResult> EditClass(int id)
    {
        try
        {
            if (!IsUserAuthenticated())
                return RedirectToLogin();

            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid class ID";
                return RedirectToAction(nameof(Index));
            }

            var classEntity = await _classService.GetClassByIdAsync(id);
            
            if (classEntity == null)
            {
                _logger.LogWarning("Class not found for editing. ID: {ClassId}", id);
                TempData["ErrorMessage"] = "Class not found";
                return RedirectToAction(nameof(Index));
            }

            // Add grade options for the dropdown with selected value
            classEntity.GradeOptions = GetGradeClassOptions(classEntity.ClassName);

            return View("~/Views/Classes/EditClass.cshtml", classEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading class for editing. ID: {ClassId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading the class";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Class/EditClass/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditClass(int id, ClassViewModel model)
    {
        try
        {
            if (!IsUserAuthenticated())
                return RedirectToLogin();

            if (id != model.Id)
            {
                _logger.LogWarning("Class ID mismatch. URL ID: {UrlId}, Model ID: {ModelId}", id, model.Id);
                TempData["ErrorMessage"] = "Class ID mismatch";
                return RedirectToAction(nameof(Index));
            }

            // Remove unnecessary properties from validation
            ModelState.Remove("GradeOptions");
            ModelState.Remove("AvailableSeats");
            ModelState.Remove("TotalStudents");
            ModelState.Remove("TotalSubjects");
            ModelState.Remove("TotalTeachers");
            ModelState.Remove("FullClassName");
            ModelState.Remove("TeacherOptions");
            ModelState.Remove("AcademicYearOptions");
            ModelState.Remove("SubjectAssignments");
            ModelState.Remove("Students");

            if (ModelState.IsValid)
            {
                var result = await _classService.UpdateClassAsync(id, model);

                if (result.Success)
                {
                    _logger.LogInformation("Class updated successfully. ID: {ClassId}, Grade: {Grade}", id, model.Grade);
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning("Failed to update class. ID: {ClassId}, Message: {Message}", id, result.Message);
                ModelState.AddModelError("", result.Message);
            }
            else
            {
                _logger.LogWarning("Model validation failed for class update. ID: {ClassId}", id);
            }

            // Repopulate grade options with the selected value preserved
            model.GradeOptions = GetGradeClassOptions(model.ClassName);
            return View("~/Views/Classes/EditClass.cshtml", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating class. ID: {ClassId}", id);
            TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
            model.GradeOptions = GetGradeClassOptions(model.ClassName);
            return View("~/Views/Classes/EditClass.cshtml", model);
        }
    }

    // POST: Class/DeleteClass/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteClass(int id)
    {
        try
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Please login to perform this action" });
            }

            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid class ID" });
            }

            var result = await _classService.DeleteClassAsync(id);

            if (result.Success)
            {
                _logger.LogInformation("Class deleted successfully. ID: {ClassId}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete class. ID: {ClassId}, Message: {Message}", id, result.Message);
            }

            return Json(new { success = result.Success, message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting class. ID: {ClassId}", id);
            return Json(new { success = false, message = "An error occurred while deleting the class" });
        }
    }

    // GET: Class/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            if (!IsUserAuthenticated())
                return RedirectToLogin();

            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid class ID";
                return RedirectToAction(nameof(Index));
            }

            var classEntity = await _classService.GetClassByIdAsync(id);
            
            if (classEntity == null)
            {
                _logger.LogWarning("Class not found. ID: {ClassId}", id);
                TempData["ErrorMessage"] = "Class not found";
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Classes/Details.cshtml", classEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error viewing class details. ID: {ClassId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading class details";
            return RedirectToAction(nameof(Index));
        }
    }
}