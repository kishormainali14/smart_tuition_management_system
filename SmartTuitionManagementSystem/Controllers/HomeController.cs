using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Data;
using SmartTuitionManagementSystem.Models;
using SmartTuitionManagementSystem.Services;

namespace SmartTuitionManagementSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // Dashboard with live data
    public async Task<IActionResult> Dashboard()
    {
        // Check if user is logged in via session
        if (HttpContext.Session.GetString("UserId") == null)
        {
            TempData["ErrorMessage"] = "Please login to access the dashboard";
            return RedirectToAction("Login", "Account");
        }

        try
        {
            // Get real-time counts from database
            var studentCount = await _context.Students.CountAsync();
            var teacherCount = await _context.Teachers.CountAsync(t => t.IsActive);
            var classCount = await _context.Classes.CountAsync();
            
            // Get monthly revenue (if Fees table exists, otherwise 0)
            var monthlyRevenue = await GetMonthlyRevenueAsync();
            
            // Get recent activities
            var recentActivities = await GetRecentActivitiesAsync();

            // Get monthly revenue data for chart (last 12 months)
            var monthlyRevenueData = await GetMonthlyRevenueDataAsync();

            // Set ViewBag data
            ViewBag.StudentCount = studentCount;
            ViewBag.TeacherCount = teacherCount;
            ViewBag.ClassCount = classCount;
            ViewBag.MonthlyRevenue = monthlyRevenue;
            ViewBag.RecentActivities = recentActivities;
            ViewBag.MonthlyRevenueData = monthlyRevenueData;
            ViewBag.UserName = HttpContext.Session.GetString("UserName") ?? "Admin";
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole") ?? "User";

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            ViewBag.StudentCount = 0;
            ViewBag.TeacherCount = 0;
            ViewBag.ClassCount = 0;
            ViewBag.MonthlyRevenue = 0;
            ViewBag.RecentActivities = new List<ActivityViewModel>();
            ViewBag.MonthlyRevenueData = new decimal[12];
            return View();
        }
    }

    private async Task<decimal> GetMonthlyRevenueAsync()
    {
        // If you have a Fees table, use this:
        // var currentMonth = DateTime.Now.Month;
        // var currentYear = DateTime.Now.Year;
        // return await _context.Fees
        //     .Where(f => f.PaymentDate.Month == currentMonth && f.PaymentDate.Year == currentYear)
        //     .SumAsync(f => f.Amount);
        
        // Placeholder - replace with actual data when Fees table is ready
        return 0;
    }

    private async Task<List<ActivityViewModel>> GetRecentActivitiesAsync()
    {
        var activities = new List<ActivityViewModel>();

        try
        {
            // Get recent student additions
            var recentStudents = await _context.Students
                .OrderByDescending(s => s.CreatedAt)
                .Take(5)
                .Select(s => new ActivityViewModel
                {
                    Icon = "fa-user-graduate",
                    IconColor = "success",
                    Title = "New Student Added",
                    Description = $"{s.FullName} was added to {s.Grade}",
                    Time = s.CreatedAt,
                    TimeAgo = GetTimeAgo(s.CreatedAt)
                })
                .ToListAsync();
            activities.AddRange(recentStudents);

            // Get recent teacher additions
            var recentTeachers = await _context.Teachers
                .Where(t => t.IsActive)
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .Select(t => new ActivityViewModel
                {
                    Icon = "fa-chalkboard-user",
                    IconColor = "primary",
                    Title = "New Teacher Added",
                    Description = $"{t.FullName} - {t.Qualification}",
                    Time = t.CreatedAt,
                    TimeAgo = GetTimeAgo(t.CreatedAt)
                })
                .ToListAsync();
            activities.AddRange(recentTeachers);

            // Get recent attendance (if any)
            var recentAttendance = await _context.TeacherAttendances
                .Include(a => a.Teacher)
                .OrderByDescending(a => a.Date)
                .Take(5)
                .Select(a => new ActivityViewModel
                {
                    Icon = "fa-calendar-check",
                    IconColor = "info",
                    Title = "Attendance Marked",
                    Description = $"{a.Teacher!.FullName} - {a.Status} on {a.Date:dd MMM yyyy}",
                    Time = a.CreatedAt,
                    TimeAgo = GetTimeAgo(a.CreatedAt)
                })
                .ToListAsync();
            activities.AddRange(recentAttendance);

            // Sort by time and take latest 10
            activities = activities.OrderByDescending(a => a.Time).Take(10).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent activities");
        }

        return activities;
    }

    private async Task<decimal[]> GetMonthlyRevenueDataAsync()
    {
        var monthlyData = new decimal[12];
        
        // If you have a Fees table, use this:
        // var currentYear = DateTime.Now.Year;
        // for (int i = 1; i <= 12; i++)
        // {
        //     monthlyData[i - 1] = await _context.Fees
        //         .Where(f => f.PaymentDate.Year == currentYear && f.PaymentDate.Month == i)
        //         .SumAsync(f => f.Amount);
        // }
        
        // Placeholder with sample data - replace when Fees table is ready
        var random = new Random();
        for (int i = 0; i < 12; i++)
        {
            monthlyData[i] = random.Next(5000, 20000);
        }
        
        return monthlyData;
    }

    private string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.Now - dateTime;

        if (timeSpan <= TimeSpan.FromSeconds(60))
            return "just now";
        if (timeSpan <= TimeSpan.FromMinutes(60))
            return $"{timeSpan.Minutes} minute{(timeSpan.Minutes > 1 ? "s" : "")} ago";
        if (timeSpan <= TimeSpan.FromHours(24))
            return $"{timeSpan.Hours} hour{(timeSpan.Hours > 1 ? "s" : "")} ago";
        if (timeSpan <= TimeSpan.FromDays(30))
            return $"{timeSpan.Days} day{(timeSpan.Days > 1 ? "s" : "")} ago";
        if (timeSpan <= TimeSpan.FromDays(365))
            return $"{timeSpan.Days / 30} month{(timeSpan.Days / 30 > 1 ? "s" : "")} ago";
        
        return $"{timeSpan.Days / 365} year{(timeSpan.Days / 365 > 1 ? "s" : "")} ago";
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class ActivityViewModel
{
    public string Icon { get; set; } = string.Empty;
    public string IconColor { get; set; } = "primary";
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
}