namespace SmartTuitionManagementSystem.Constants;

public static class AttendanceStatus
{
    public const string Present = "Present";
    public const string Absent = "Absent";
    public const string NotPresent = "NotPresent";
    
    public static readonly List<string> All = new List<string>()
    {
        Present,
        Absent,
        NotPresent
    };
}