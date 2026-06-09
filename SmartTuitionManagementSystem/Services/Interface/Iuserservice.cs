using SmartTuitionManagementSystem.Models;

namespace SmartTuitionManagementSystem.Services
{
    public interface IUserService
    {
        // Authentication methods
        Task<(bool Success, string Message, string UserId)> RegisterUserAsync(RegisterViewModel model);
        Task<(bool Success, string Message)> LoginUserAsync(LoginViewModel model, bool rememberMe);
        Task LogoutUserAsync();
        
        // User lookup methods
        Task<UserViewModel?> GetUserByEmailAsync(string email);
        Task<UserViewModel?> GetUserByUsernameAsync(string username);
        Task<UserViewModel?> GetUserByIdAsync(int userId);
        
        // Availability check methods
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<bool> IsUsernameAvailableAsync(string username);
        Task<bool> IsEmailAvailableAsync(string email);
        
        // Admin user management methods
        Task<List<UserViewModel>> GetAllUsersAsync();
        Task<ServiceResult> UpdateUserAsync(UserViewModel model);
        Task<ServiceResult> DeleteUserAsync(int userId);
    }
    
    // Helper class for consistent service responses
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}