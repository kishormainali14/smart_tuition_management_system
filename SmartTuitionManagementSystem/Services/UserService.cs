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

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public async Task<(bool Success, string Message, string UserId)> RegisterUserAsync(RegisterViewModel model)
        {
            try
            {
                if (await IsEmailExistsAsync(model.Email))
                {
                    return (false, "User with this email already exists", null);
                }

                if (await IsUsernameExistsAsync(model.Username))
                {
                    return (false, "Username already taken. Please choose another.", null);
                }

                bool isFirstUser = !await _context.Users.AnyAsync();

                var user = new UserEntity
                {
                    Name = model.Name,
                    Email = model.Email,
                    Username = model.Username,
                    Address = model.Address,
                    Contact = model.Contact,
                    PasswordHash = HashPassword(model.Password),
                    IsActive = true,
                    Role = isFirstUser ? "Admin" : "User",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

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

        public async Task<(bool Success, string Message)> LoginUserAsync(LoginViewModel model, bool rememberMe)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                
                if (user == null)
                {
                    return (false, "Invalid email or password");
                }

                if (!user.IsActive)
                {
                    return (false, "Your account is deactivated. Please contact administrator.");
                }

                var hashedPassword = HashPassword(model.Password);
                if (user.PasswordHash != hashedPassword)
                {
                    return (false, "Invalid email or password");
                }

                user.LastLoginDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _httpContextAccessor.HttpContext?.Session.SetString("UserId", user.Id.ToString());
                _httpContextAccessor.HttpContext?.Session.SetString("Username", user.Username);
                _httpContextAccessor.HttpContext?.Session.SetString("UserEmail", user.Email);
                _httpContextAccessor.HttpContext?.Session.SetString("UserName", user.Name);
                _httpContextAccessor.HttpContext?.Session.SetString("UserRole", user.Role ?? "User");

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

        public async Task<UserViewModel?> GetUserByEmailAsync(string email)
        {
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

        public async Task<UserViewModel?> GetUserByUsernameAsync(string username)
        {
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

        public async Task<UserViewModel?> GetUserByIdAsync(int userId)
        {
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

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            return !await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<List<UserViewModel>> GetAllUsersAsync()
        {
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

        public async Task<ServiceResult> UpdateUserAsync(UserViewModel model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);
                if (user == null)
                {
                    return new ServiceResult { Success = false, Message = "User not found." };
                }

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

        public async Task<ServiceResult> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return new ServiceResult { Success = false, Message = "User not found." };
                }

                var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin");
                if (user.Role == "Admin" && adminCount <= 1)
                {
                    return new ServiceResult { Success = false, Message = "Cannot delete the last admin user." };
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return new ServiceResult { Success = true, Message = "User deleted successfully!" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error deleting user: {ex.Message}" };
            }
        }

        public async Task LogoutUserAsync()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete("UserEmail");
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(".AspNetCore.Session");
            await Task.CompletedTask;
        }

        // ========== SIMPLE PASSWORD RESET METHODS ==========

        // Step 1: Verify user exists by email
        public async Task<(bool Success, string Message, int? UserId)> VerifyUserForResetAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
                
                if (user == null)
                {
                    return (false, "No account found with this email address.", null);
                }

                return (true, "User verified. Please enter your new password.", user.Id);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}", null);
            }
        }

        // Step 2: Reset password directly
        public async Task<(bool Success, string Message)> ResetPasswordDirectAsync(int userId, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                
                if (user == null)
                {
                    return (false, "User not found.");
                }

                // Update password
                user.PasswordHash = HashPassword(newPassword);
                await _context.SaveChangesAsync();

                return (true, "Password has been reset successfully! Please login with your new password.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }
    }
}