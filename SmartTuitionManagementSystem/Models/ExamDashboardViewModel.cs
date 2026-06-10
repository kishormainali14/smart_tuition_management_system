namespace SmartTuitionManagementSystem.Models;

public class ExamDashboardViewModel
{
    public int TotalExams { get; set; }
    public int ActiveExams { get; set; }
    public int InactiveExams { get; set; }
    public int UpcomingExams { get; set; }
    public int OngoingExams { get; set; }
    public int CompletedExams { get; set; }

    // Calculated Properties
    public double ActivePercentage => TotalExams > 0 ? (double)ActiveExams / TotalExams * 100 : 0;
    public double CompletionRate => TotalExams > 0 ? (double)CompletedExams / TotalExams * 100 : 0;
}