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

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetString("UserId") != null)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        return View();
    }

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

    [HttpGet]
    public IActionResult Register()
    {
        TempData["ErrorMessage"] = "Public registration is disabled. Contact an administrator.";
        return RedirectToAction("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        TempData["ErrorMessage"] = "Public registration is disabled. Contact an administrator.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AddUser()
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            TempData["ErrorMessage"] = "Access denied. Admin privileges required.";
            return RedirectToAction("Dashboard", "Home");
        }
        return View("~/Views/UserManagement/AddUser.cshtml", new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddUser(RegisterViewModel model)
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            TempData["ErrorMessage"] = "Access denied. Admin privileges required.";
            return RedirectToAction("Dashboard", "Home");
        }

        if (model == null)
        {
            TempData["ErrorMessage"] = "No data received";
            return View("~/Views/UserManagement/AddUser.cshtml", model);
        }

        _logger.LogInformation($"Admin AddUser attempt for Email: {model.Email}, Username: {model.Username}");

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            _logger.LogWarning($"ModelState invalid: {string.Join(", ", errors)}");
            TempData["ErrorMessage"] = "Please fix the validation errors.";
            return View("~/Views/UserManagement/AddUser.cshtml", model);
        }

        try
        {
            var result = await _userService.RegisterUserAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = $"User '{model.Username}' created successfully!";
                _logger.LogInformation($"Admin created user: {model.Email}");
                return RedirectToAction("Users");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                TempData["ErrorMessage"] = result.Message;
                _logger.LogWarning($"Admin user creation failed: {result.Message}");
                return View("~/Views/UserManagement/AddUser.cshtml", model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception during admin user creation for {model.Email}");
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            return View("~/Views/UserManagement/AddUser.cshtml", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Users()
    {
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _userService.LogoutUserAsync();
        TempData["SuccessMessage"] = "You have been logged out successfully.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    // ========== SIMPLE PASSWORD RESET ACTIONS ==========

    // GET: /Account/ForgotPassword
    [HttpGet]
    public IActionResult ForgetPassword()
    {
        return View();
    }

    // POST: /Account/ForgotPassword - Verify email and go to reset page
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgetPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _userService.VerifyUserForResetAsync(model.Email);

        if (result.Success && result.UserId.HasValue)
        {
            // Store UserId in TempData for the reset page
            TempData["ResetUserId"] = result.UserId.Value;
            TempData["ResetEmail"] = model.Email;
            return RedirectToAction("ResetPassword");
        }

        TempData["ErrorMessage"] = result.Message;
        return View(model);
    }

    // GET: /Account/ResetPassword
    [HttpGet]
    public IActionResult ResetPassword()
    {
        // Check if we have a user ID from the forgot password step
        if (TempData["ResetUserId"] == null)
        {
            TempData["ErrorMessage"] = "Please request a password reset first.";
            return RedirectToAction("ForgetPassword");
        }

        var model = new ResetPasswordViewModel
        {
            UserId = (int)(TempData["ResetUserId"] ?? throw new InvalidOperationException()),
            Email = TempData["ResetEmail"]?.ToString() ?? string.Empty
        };

        // Keep TempData for the POST request
        TempData.Keep("ResetUserId");
        TempData.Keep("ResetEmail");

        return View(model);
    }

    // POST: /Account/ResetPassword - Save new password
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Verify passwords match
        if (model.NewPassword != model.ConfirmPassword)
        {
            ModelState.AddModelError("", "New password and confirmation password do not match.");
            return View(model);
        }

        // Validate password length
        if (model.NewPassword.Length < 6)
        {
            ModelState.AddModelError("", "Password must be at least 6 characters long.");
            return View(model);
        }

        var result = await _userService.ResetPasswordDirectAsync(model.UserId, model.NewPassword);

        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Login");
        }

        ModelState.AddModelError("", result.Message);
        return View(model);
    }
}