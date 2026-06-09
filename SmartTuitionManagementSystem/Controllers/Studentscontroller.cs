using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTuitionManagementSystem.Models;
using SmartTuitionManagementSystem.Services;

namespace SmartTuitionManagementSystem.Controllers;

public class StudentsController : Controller
{
    private readonly ILogger<StudentsController> _logger;
    private readonly IStudentService _studentService;

    public StudentsController(ILogger<StudentsController> logger, IStudentService studentService)
    {
        _logger = logger;
        _studentService = studentService;
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

    // GET: Students/AddStudent
    [HttpGet]
    public IActionResult AddStudent()
    {
        try
        {
            if (!IsUserAuthenticated())
                return RedirectToLogin();

            var model = new StudentViewModel
            {
                GradeOptions = GetGradeClassOptions(),
                EnrollmentDate = DateTime.Now,
                IsActive = true
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading AddStudent page");
            TempData["ErrorMessage"] = "An error occurred while loading the page";
            return RedirectToAction("StudentList");
        }
    }

    // POST: Students/AddStudent
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddStudent(StudentViewModel model)
    {
        try
        {
            if (!IsUserAuthenticated())
                return RedirectToLogin();

            // Remove unnecessary validation for fields that aren't in the form
            ModelState.Remove("GradeOptions");
            ModelState.Remove("EnrollmentDate");
            ModelState.Remove("IsActive");

            // Set server-side values
            model.EnrollmentDate = DateTime.Now;
            model.IsActive = true;

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed for student addition");
                
                model.GradeOptions = GetGradeClassOptions();
                return View(model);
            }

            // Additional business validation
            if (model.DateOfBirth.HasValue && model.DateOfBirth.Value > DateTime.Now.AddYears(-3))
            {
                ModelState.AddModelError("DateOfBirth", "Student must be at least 3 years old");
                model.GradeOptions = GetGradeClassOptions();
                return View(model);
            }

            var result = await _studentService.AddStudentAsync(model);

            if (result.Success)
            {
                _logger.LogInformation("Student added successfully. StudentId: {StudentId}, Email: {Email}", 
                    result.StudentId, model.Email);
                
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(StudentList));
            }

            _logger.LogWarning("Failed to add student: {Message}", result.Message);
            ModelState.AddModelError("", result.Message);
            model.GradeOptions = GetGradeClassOptions();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding student. Email: {Email}", model.Email);
            TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
            model.GradeOptions = GetGradeClassOptions();
            return View(model);
        }
    }

    // GET: Students/StudentList
    [HttpGet]
    public async Task<IActionResult> StudentList(string searchTerm = "")
    {
        try
        {
            if (!IsUserAuthenticated())
                return RedirectToLogin();

            ViewBag.SearchTerm = searchTerm;

            var students = await _studentService.GetAllStudentsAsync();
            
            // Apply search filter if needed
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                students = students.Where(s => 
                    s.FullName.ToLower().Contains(searchTerm) ||
                    s.Email.ToLower().Contains(searchTerm) ||
                    s.PhoneNumber.Contains(searchTerm) ||
                    s.Grade.ToLower().Contains(searchTerm) ||
                    (s.ParentName != null && s.ParentName.ToLower().Contains(searchTerm))).ToList();
            }
            
            ViewBag.TotalCount = students.Count;
            
            return View(students);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading student list");
            TempData["ErrorMessage"] = "An error occurred while loading the student list";
            return View(new List<StudentViewModel>());
        }
    }

    // GET: Students/EditStudent/5
    [HttpGet]
    public async Task<IActionResult> EditStudent(int id)
    {
        try
        {
            if (!IsUserAuthenticated())
                return RedirectToLogin();

            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid student ID";
                return RedirectToAction(nameof(StudentList));
            }

            var student = await _studentService.GetStudentByIdAsync(id);
            
            if (student == null)
            {
                _logger.LogWarning("Student not found for editing. ID: {StudentId}", id);
                TempData["ErrorMessage"] = "Student not found";
                return RedirectToAction(nameof(StudentList));
            }

            student.GradeOptions = GetGradeClassOptions();
            return View(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading student for editing. ID: {StudentId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading the student";
            return RedirectToAction(nameof(StudentList));
        }
    }

    // POST: Students/EditStudent/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditStudent(int id, StudentViewModel model)
    {
        try
        {
            if (!IsUserAuthenticated())
                return RedirectToLogin();

            if (id != model.Id)
            {
                _logger.LogWarning("Student ID mismatch. URL ID: {UrlId}, Model ID: {ModelId}", id, model.Id);
                TempData["ErrorMessage"] = "Student ID mismatch";
                return RedirectToAction(nameof(StudentList));
            }

            // Remove unnecessary validation
            ModelState.Remove("GradeOptions");
            ModelState.Remove("EnrollmentDate");
            ModelState.Remove("IsActive");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed for student update. ID: {StudentId}", id);
                model.GradeOptions = GetGradeClassOptions();
                return View(model);
            }

            // Additional business validation
            if (model.DateOfBirth.HasValue && model.DateOfBirth.Value > DateTime.Now.AddYears(-3))
            {
                ModelState.AddModelError("DateOfBirth", "Student must be at least 3 years old");
                model.GradeOptions = GetGradeClassOptions();
                return View(model);
            }

            var result = await _studentService.UpdateStudentAsync(id, model);

            if (result.Success)
            {
                _logger.LogInformation("Student updated successfully. ID: {StudentId}, Email: {Email}", id, model.Email);
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(StudentList));
            }

            _logger.LogWarning("Failed to update student. ID: {StudentId}, Message: {Message}", id, result.Message);
            ModelState.AddModelError("", result.Message);
            model.GradeOptions = GetGradeClassOptions();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating student. ID: {StudentId}", id);
            TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
            model.GradeOptions = GetGradeClassOptions();
            return View(model);
        }
    }

    // POST: Students/DeleteStudent/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        try
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Please login to perform this action" });
            }

            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid student ID" });
            }

            var result = await _studentService.DeleteStudentAsync(id);

            if (result.Success)
            {
                _logger.LogInformation("Student deleted successfully. ID: {StudentId}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete student. ID: {StudentId}, Message: {Message}", id, result.Message);
            }

            return Json(new { success = result.Success, message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student. ID: {StudentId}", id);
            return Json(new { success = false, message = "An error occurred while deleting the student" });
        }
    }

    // GET: Students/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            if (!IsUserAuthenticated())
                return RedirectToLogin();

            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid student ID";
                return RedirectToAction(nameof(StudentList));
            }

            var student = await _studentService.GetStudentByIdAsync(id);
            
            if (student == null)
            {
                _logger.LogWarning("Student not found. ID: {StudentId}", id);
                TempData["ErrorMessage"] = "Student not found";
                return RedirectToAction(nameof(StudentList));
            }

            return View(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error viewing student details. ID: {StudentId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading student details";
            return RedirectToAction(nameof(StudentList));
        }
    }

    // GET: Students/ExportToExcel
    [HttpGet]
    public async Task<IActionResult> ExportToExcel()
    {
        try
        {
            if (!IsUserAuthenticated())
                return RedirectToLogin();

            var students = await _studentService.GetAllStudentsAsync();
            
            if (!students.Any())
            {
                TempData["ErrorMessage"] = "No students found to export";
                return RedirectToAction(nameof(StudentList));
            }

            // Build CSV content
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("ID,Full Name,Email,Phone Number,Grade,Parent Name,Parent Phone,Address,Enrollment Date");
            
            foreach (var student in students)
            {
                csv.AppendLine($"\"{student.Id}\",\"{student.FullName}\",\"{student.Email}\",\"{student.PhoneNumber}\",\"{student.Grade}\",\"{student.ParentName}\",\"{student.ParentPhone}\",\"{student.Address}\",\"{student.EnrollmentDate:yyyy-MM-dd}\"");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            var result = File(bytes, "text/csv", $"Students_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            
            _logger.LogInformation("Student data exported to CSV. Count: {Count}", students.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting student data");
            TempData["ErrorMessage"] = "An error occurred while exporting data";
            return RedirectToAction(nameof(StudentList));
        }
    }

    // GET: Students/Index - Redirect to StudentList
    public IActionResult Index()
    {
        return RedirectToAction(nameof(StudentList));
    }

    // Helper method to get grade/class options for dropdown
    private IEnumerable<SelectListItem> GetGradeClassOptions()
    {
        return new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "-- Select Grade --", Disabled = true, Selected = true },
            new SelectListItem { Value = "Nursery", Text = "Nursery" },
            new SelectListItem { Value = "LKG", Text = "LKG" },
            new SelectListItem { Value = "UKG", Text = "UKG" },
            new SelectListItem { Value = "Class 1", Text = "Class 1" },
            new SelectListItem { Value = "Class 2", Text = "Class 2" },
            new SelectListItem { Value = "Class 3", Text = "Class 3" },
            new SelectListItem { Value = "Class 4", Text = "Class 4" },
            new SelectListItem { Value = "Class 5", Text = "Class 5" },
            new SelectListItem { Value = "Class 6", Text = "Class 6" },
            new SelectListItem { Value = "Class 7", Text = "Class 7" },
            new SelectListItem { Value = "Class 8", Text = "Class 8" },
            new SelectListItem { Value = "Class 9", Text = "Class 9" },
            new SelectListItem { Value = "Class 10", Text = "Class 10" },
            new SelectListItem { Value = "Class 11", Text = "Class 11" },
            new SelectListItem { Value = "Class 12", Text = "Class 12" }
        };
    }
}