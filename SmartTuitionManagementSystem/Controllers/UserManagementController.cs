using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Data;
using SmartTuitionManagementSystem.Entities;
using SmartTuitionManagementSystem.Models;
using SmartTuitionManagementSystem.Services;
using System.Security.Cryptography;
using System.Text;

namespace SmartTuitionManagementSystem.Controllers;

public class UserManagementController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserManagementController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserManagementController(
        ApplicationDbContext context, 
        ILogger<UserManagementController> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    // Hash password using SHA256
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    // GET: UserManagement/Index (List all users)
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // Check if user is admin
        var userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            TempData["ErrorMessage"] = "Access denied. Admin privileges required.";
            return RedirectToAction("Dashboard", "Home");
        }

        try
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new UserViewModel
                {
                    UserId = u.Id,
                    Username = u.Username,
                    Name = u.Name,
                    Email = u.Email,
                    Contact = u.Contact,
                    Address = u.Address,
                    Role = u.Role ?? "User",
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    LastLoginDate = u.LastLoginDate
                })
                .ToListAsync();

            return View("UserList", users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users list");
            TempData["ErrorMessage"] = "Error loading users list.";
            return View("UserList", new List<UserViewModel>());
        }
    }

    // GET: UserManagement/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        // Check if user is admin
        var userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            TempData["ErrorMessage"] = "Access denied. Admin privileges required.";
            return RedirectToAction("Dashboard", "Home");
        }

        try
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserViewModel
                {
                    UserId = u.Id,
                    Username = u.Username,
                    Name = u.Name,
                    Email = u.Email,
                    Contact = u.Contact,
                    Address = u.Address,
                    Role = u.Role ?? "User",
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    LastLoginDate = u.LastLoginDate
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index");
            }

            return View(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving user details for ID: {id}");
            TempData["ErrorMessage"] = "Error loading user details.";
            return RedirectToAction("Index");
        }
    }

    // GET: UserManagement/Create
    [HttpGet]
    public IActionResult Create()
    {
        // Check if user is admin
        var userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            TempData["ErrorMessage"] = "Access denied. Admin privileges required.";
            return RedirectToAction("Dashboard", "Home");
        }

        return RedirectToAction("AddUser", "Account");
    }

    // POST: UserManagement/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RegisterViewModel model)
    {
        // Check if user is admin
        var userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            TempData["ErrorMessage"] = "Access denied. Admin privileges required.";
            return RedirectToAction("Dashboard", "Home");
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View("AddUser", model);
                }

                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    return View("AddUser", model);
                }

                // Create new user
                var user = new UserEntity
                {
                    Name = model.Name,
                    Email = model.Email,
                    Username = model.Username,
                    PasswordHash = HashPassword(model.Password),
                    Contact = model.Contact,
                    Address = model.Address,
                    Role = "User", // Default role
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"User '{model.Username}' created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                ModelState.AddModelError("", $"Error creating user: {ex.Message}");
            }
        }

        return View("AddUser", model);
    }

    // GET: UserManagement/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        // Check if user is admin
        var userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            TempData["ErrorMessage"] = "Access denied. Admin privileges required.";
            return RedirectToAction("Dashboard", "Home");
        }

        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index");
            }

            var model = new UserViewModel
            {
                UserId = user.Id,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                Contact = user.Contact,
                Address = user.Address,
                Role = user.Role ?? "User",
                IsActive = user.IsActive
            };

            return View("EditUser", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving user for edit: {id}");
            TempData["ErrorMessage"] = "Error loading user data.";
            return RedirectToAction("Index");
        }
    }

    // POST: UserManagement/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserViewModel model)
    {
        // Check if user is admin
        var userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            TempData["ErrorMessage"] = "Access denied. Admin privileges required.";
            return RedirectToAction("Dashboard", "Home");
        }

        if (id != model.UserId)
        {
            TempData["ErrorMessage"] = "User ID mismatch.";
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("Index");
                }

                // Update user properties
                user.Name = model.Name;
                user.Email = model.Email;
                user.Contact = model.Contact;
                user.Address = model.Address;
                user.Role = model.Role;
                user.IsActive = model.IsActive;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExistsAsync(id))
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("Index");
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user: {id}");
                ModelState.AddModelError("", $"Error updating user: {ex.Message}");
            }
        }

        return View("EditUser", model);
    }

    // POST: UserManagement/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        // Check if user is admin
        var userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            return Json(new { success = false, message = "Access denied. Admin privileges required." });
        }

        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Prevent deleting your own account
            var currentUserId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
            if (currentUserId == id.ToString())
            {
                return Json(new { success = false, message = "You cannot delete your own account." });
            }

            // Prevent deleting the last admin
            var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin");
            if (user.Role == "Admin" && adminCount <= 1)
            {
                return Json(new { success = false, message = "Cannot delete the last admin user." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "User deleted successfully!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting user: {id}");
            return Json(new { success = false, message = $"Error deleting user: {ex.Message}" });
        }
    }

    // POST: UserManagement/ToggleStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        // Check if user is admin
        var userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            return Json(new { success = false, message = "Access denied. Admin privileges required." });
        }

        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Prevent deactivating your own account
            var currentUserId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
            if (currentUserId == id.ToString())
            {
                return Json(new { success = false, message = "You cannot deactivate your own account." });
            }

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            var status = user.IsActive ? "activated" : "deactivated";
            return Json(new { success = true, message = $"User {status} successfully!", isActive = user.IsActive });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error toggling user status: {id}");
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    // GET: UserManagement/Search
    [HttpGet]
    public async Task<IActionResult> Search(string searchTerm)
    {
        // Check if user is admin
        var userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            return Json(new { success = false, message = "Access denied." });
        }

        try
        {
            var users = await _context.Users
                .Where(u => string.IsNullOrEmpty(searchTerm) ||
                    u.Name.Contains(searchTerm) ||
                    u.Email.Contains(searchTerm) ||
                    u.Username.Contains(searchTerm))
                .Select(u => new UserViewModel
                {
                    UserId = u.Id,
                    Username = u.Username,
                    Name = u.Name,
                    Email = u.Email,
                    Contact = u.Contact,
                    Address = u.Address,
                    Role = u.Role ?? "User",
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return Json(new { success = true, data = users });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users");
            return Json(new { success = false, message = "Error searching users." });
        }
    }

    // GET: UserManagement/Export (Export users to CSV)
    [HttpGet]
    public async Task<IActionResult> Export()
    {
        // Check if user is admin
        var userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            TempData["ErrorMessage"] = "Access denied. Admin privileges required.";
            return RedirectToAction("Dashboard", "Home");
        }

        try
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("ID,Username,Name,Email,Contact,Address,Role,IsActive,CreatedAt,LastLoginDate");

            foreach (var user in users)
            {
                csv.AppendLine($"{user.Id},{user.Username},{user.Name},{user.Email},{user.Contact},{user.Address},{user.Role},{user.IsActive},{user.CreatedAt},{user.LastLoginDate}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"Users_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting users");
            TempData["ErrorMessage"] = "Error exporting users.";
            return RedirectToAction("Index");
        }
    }

    private async Task<bool> UserExistsAsync(int id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }
}