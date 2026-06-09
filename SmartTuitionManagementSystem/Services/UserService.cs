using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Data;
using SmartTuitionManagementSystem.Entities;
using SmartTuitionManagementSystem.Models;
using System.Security.Cryptography;
using System.Text;

namespace SmartTuitionManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
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

        // Register User - Takes data from controller and saves to database
// Register User - Takes data from controller and saves to database
public async Task<(bool Success, string Message, string UserId)> RegisterUserAsync(RegisterViewModel model)
{
    try
    {
        // Check if email already exists
        if (await IsEmailExistsAsync(model.Email))
        {
            return (false, "User with this email already exists", null);
        }

        // Check if username already exists
        if (await IsUsernameExistsAsync(model.Username))
        {
            return (false, "Username already taken. Please choose another.", null);
        }

        // ✅ ADD THIS: Check if this is the first user (no users in database)
        bool isFirstUser = !await _context.Users.AnyAsync();

        // Create new user entity
        var user = new UserEntity
        {
            Name = model.Name,
            Email = model.Email,
            Username = model.Username,
            Address = model.Address,
            Contact = model.Contact,
            PasswordHash = HashPassword(model.Password),
            IsActive = true,
            Role = isFirstUser ? "Admin" : "User", // ✅ First user becomes Admin
            CreatedAt = DateTime.UtcNow
        };

        // Save to database
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Set session
        _httpContextAccessor.HttpContext?.Session.SetString("UserId", user.Id.ToString());
        _httpContextAccessor.HttpContext?.Session.SetString("Username", user.Username);
        _httpContextAccessor.HttpContext?.Session.SetString("UserEmail", user.Email);
        _httpContextAccessor.HttpContext?.Session.SetString("UserRole", user.Role);
        _httpContextAccessor.HttpContext?.Session.SetString("UserName", user.Name);

        return (true, "Registration successful!", user.Id.ToString());
    }
    catch (Exception ex)
    {
        return (false, $"An error occurred: {ex.Message}", null);
    }
}
        // Login User - Validates credentials from database
        public async Task<(bool Success, string Message)> LoginUserAsync(LoginViewModel model, bool rememberMe)
        {
            try
            {
                // ✅ CHANGED: _context.User → _context.Users
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                
                if (user == null)
                {
                    return (false, "Invalid email or password");
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    return (false, "Your account is deactivated. Please contact administrator.");
                }

                // Verify password
                var hashedPassword = HashPassword(model.Password);
                if (user.PasswordHash != hashedPassword)
                {
                    return (false, "Invalid email or password");
                }

                // Update last login date
                user.LastLoginDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Set session
                _httpContextAccessor.HttpContext?.Session.SetString("UserId", user.Id.ToString());
                _httpContextAccessor.HttpContext?.Session.SetString("Username", user.Username);
                _httpContextAccessor.HttpContext?.Session.SetString("UserEmail", user.Email);
                _httpContextAccessor.HttpContext?.Session.SetString("UserName", user.Name);
                _httpContextAccessor.HttpContext?.Session.SetString("UserRole", user.Role ?? "User");

                // Set cookie if remember me
                if (rememberMe)
                {
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7),
                        HttpOnly = true,
                        IsEssential = true
                    };
                    _httpContextAccessor.HttpContext?.Response.Cookies.Append("UserEmail", user.Email, cookieOptions);
                }

                return (true, "Login successful!");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        // Get user by email from database
        public async Task<UserViewModel?> GetUserByEmailAsync(string email)
        {
            // ✅ CHANGED: _context.User → _context.Users
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            return new UserViewModel
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Username = user.Username,
                Address = user.Address,
                Contact = user.Contact,
                IsActive = user.IsActive,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                LastLoginDate = user.LastLoginDate
            };
        }

        // Get user by username
        public async Task<UserViewModel?> GetUserByUsernameAsync(string username)
        {
            // ✅ CHANGED: _context.User → _context.Users
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            return new UserViewModel
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Username = user.Username,
                Address = user.Address,
                Contact = user.Contact,
                IsActive = user.IsActive,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                LastLoginDate = user.LastLoginDate
            };
        }

        // Get user by ID
        public async Task<UserViewModel?> GetUserByIdAsync(int userId)
        {
            // ✅ CHANGED: _context.User → _context.Users
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            return new UserViewModel
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Username = user.Username,
                Address = user.Address,
                Contact = user.Contact,
                IsActive = user.IsActive,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                LastLoginDate = user.LastLoginDate
            };
        }

        // Check if email exists in database
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            // ✅ CHANGED: _context.User → _context.Users
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        // Check if username exists
        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            // ✅ CHANGED: _context.User → _context.Users
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        // Check if username is available
        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            // ✅ CHANGED: _context.User → _context.Users
            return !await _context.Users.AnyAsync(u => u.Username == username);
        }

        // Check if email is available
        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            // ✅ CHANGED: _context.User → _context.Users
            return !await _context.Users.AnyAsync(u => u.Email == email);
        }

        // Get all users for admin
        public async Task<List<UserViewModel>> GetAllUsersAsync()
        {
            // ✅ CHANGED: _context.User → _context.Users
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return users.Select(u => new UserViewModel
            {
                UserId = u.Id,
                Username = u.Username,
                Name = u.Name,
                Email = u.Email,
                Contact = u.Contact,
                Address = u.Address,
                Role = u.Role ?? "User",
                IsActive = u.IsActive
            }).ToList();
        }

        // Update user (admin function)
        public async Task<ServiceResult> UpdateUserAsync(UserViewModel model)
        {
            try
            {
                // ✅ CHANGED: _context.User → _context.Users
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);
                if (user == null)
                {
                    return new ServiceResult { Success = false, Message = "User not found." };
                }

                // Update user properties
                user.Name = model.Name;
                user.Email = model.Email;
                user.Contact = model.Contact;
                user.Address = model.Address;
                user.Role = model.Role;
                user.IsActive = model.IsActive;

                await _context.SaveChangesAsync();

                return new ServiceResult { Success = true, Message = "User updated successfully!" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error updating user: {ex.Message}" };
            }
        }

        // Delete user (admin function)
        public async Task<ServiceResult> DeleteUserAsync(int userId)
        {
            try
            {
                // ✅ CHANGED: _context.User → _context.Users
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return new ServiceResult { Success = false, Message = "User not found." };
                }

                // Prevent deleting the last admin user
                // ✅ CHANGED: _context.User → _context.Users
                var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin");
                if (user.Role == "Admin" && adminCount <= 1)
                {
                    return new ServiceResult { Success = false, Message = "Cannot delete the last admin user." };
                }

                // ✅ CHANGED: _context.User → _context.Users
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return new ServiceResult { Success = true, Message = "User deleted successfully!" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error deleting user: {ex.Message}" };
            }
        }

        // Logout User
        public async Task LogoutUserAsync()
        {
            // Clear all session data
            _httpContextAccessor.HttpContext?.Session.Clear();
    
            // Delete cookies
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete("UserEmail");
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(".AspNetCore.Session");
    
            await Task.CompletedTask;
        }
    }
}