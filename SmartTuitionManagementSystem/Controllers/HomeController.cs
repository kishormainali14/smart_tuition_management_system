using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartTuitionManagementSystem.Models;
using SmartTuitionManagementSystem.Services;

namespace SmartTuitionManagementSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserService _userService;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
       // _userService = userService;
          
    }

    // Remove [Authorize] - use custom check instead
    public IActionResult Dashboard()
    {
       // var studentCount = _context.Students.Count();
      //  ViewBag.StudentCount = studentCount;
        // Check if user is logged in via session
        if (HttpContext.Session.GetString("UserId") == null)
        {
            TempData["ErrorMessage"] = "Please login to access the dashboard";
            return RedirectToAction("Login", "Account");
        }
        
        return View();
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