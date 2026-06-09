using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartTuitionManagementSystem.Models;
using SmartTuitionManagementSystem.Services;

namespace SmartTuitionManagementSystem.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IUserService _userService;

    public AccountController(ILogger<AccountController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetString("UserId") != null)
        {
            return RedirectToAction("Dashboard", "Home");
        }

        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _userService.LoginUserAsync(model, model.RememberMe);

        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Dashboard", "Home");
        }

        ModelState.AddModelError("", result.Message);
        return View(model);
    }

    // GET: /Account/Register (For self-registration)
    [HttpGet]
    public IActionResult Register()
    {
        // Check if user is already logged in
        if (HttpContext.Session.GetString("UserId") != null)
        {
            return RedirectToAction("Dashboard", "Home");
        }

        return View();
    }

    // POST: /Account/Register (For self-registration)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (model == null)
        {
            TempData["ErrorMessage"] = "No data received";
            return View(model);
        }

        _logger.LogInformation($"Register attempt for Email: {model.Email}, Username: {model.Username}");

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            _logger.LogWarning($"ModelState invalid: {string.Join(", ", errors)}");
            return View(model);
        }

        try
        {
            var result = await _userService.RegisterUserAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                _logger.LogInformation($"Registration successful for {model.Email}");
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                TempData["ErrorMessage"] = result.Message;
                _logger.LogWarning($"Registration failed: {result.Message}");
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception during registration for {model.Email}");
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            return View(model);
        }
    }

    // GET: /Account/AddUser (For admin to add users)
    [HttpGet]
    public IActionResult AddUser()
    {
        // Check if user is admin
        var userRole = HttpContext.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            TempData["ErrorMessage"] = "Access denied. Admin privileges required.";
            return RedirectToAction("Dashboard", "Home");
        }

        return View("Admin", new RegisterViewModel());
    }

    // POST: /Account/AddUser (Admin creates a new user)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddUser(RegisterViewModel model)
    {
        // Check if user is admin
        var userRole = HttpContext.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            TempData["ErrorMessage"] = "Access denied. Admin privileges required.";
            return RedirectToAction("Dashboard", "Home");
        }

        if (model == null)
        {
            TempData["ErrorMessage"] = "No data received";
            return View("Admin", model);
        }

        _logger.LogInformation($"Admin AddUser attempt for Email: {model.Email}, Username: {model.Username}");

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            _logger.LogWarning($"ModelState invalid: {string.Join(", ", errors)}");
            TempData["ErrorMessage"] = "Please fix the validation errors.";
            return View("Admin", model);
        }

        try
        {
            var result = await _userService.RegisterUserAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = $"User '{model.Username}' created successfully!";
                _logger.LogInformation($"Admin created user: {model.Email}");

                // Redirect back to Users list after successful creation
                return RedirectToAction("Users");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                TempData["ErrorMessage"] = result.Message;
                _logger.LogWarning($"Admin user creation failed: {result.Message}");
                return View("Admin", model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception during admin user creation for {model.Email}");
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            return View("Admin", model);
        }
    }

    // GET: /Account/Users (List all users - Admin only)
    [HttpGet]
    public async Task<IActionResult> Users()
    {
        // Check if user is admin
        var userRole = HttpContext.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            TempData["ErrorMessage"] = "Access denied. Admin privileges required.";
            return RedirectToAction("Dashboard", "Home");
        }

        try
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users list");
            TempData["ErrorMessage"] = "Error loading users list.";
            return View(new List<UserViewModel>());
        }
    }


// GET: /Account/CheckUsernameAvailability
    [HttpGet]
    public async Task<IActionResult> CheckUsernameAvailability(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return Json(new { available = false });
        }

        try
        {
            var isAvailable = await _userService.IsUsernameAvailableAsync(username);
            return Json(new { available = isAvailable });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking username availability: {username}");
            return Json(new { available = false, error = true });
        }
    }

    // GET: /Account/CheckEmailAvailability
    [HttpGet]
    public async Task<IActionResult> CheckEmailAvailability(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Json(new { available = false });
        }

        try
        {
            var isAvailable = await _userService.IsEmailAvailableAsync(email);
            return Json(new { available = isAvailable });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking email availability: {email}");
            return Json(new { available = false, error = true });
        }
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _userService.LogoutUserAsync();
        TempData["SuccessMessage"] = "You have been logged out successfully.";
        return RedirectToAction("Login");
    }

    // GET: /Account/AccessDenied
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    // GET: /Account/ForgotPassword
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    // POST: /Account/ForgotPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _logger.LogInformation("Password reset requested for email '{Email}'.", model.Email);
        TempData["SuccessMessage"] = "If that email is registered, a password reset link has been sent.";
        return RedirectToAction("Login");
    }
}