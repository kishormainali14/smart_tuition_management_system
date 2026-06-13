# Fix Attendance Save & Report

## Problem
SaveAttendance POST handler's `ModelState.IsValid` check prevents save from executing — no TempData message is set, data never persisted, report shows 0%.

## Root Cause
`ModelState.IsValid` fails because `[Required]` on `int SelectedClassId` considers `0` invalid. When the form is submitted, the model binding likely receives `0` for `SelectedClassId` (or another validation issue), causing the handler to return the view directly without ever calling `SaveAttendanceAsync` or setting TempData messages. The view has no validation summary, so the user sees nothing.

The `ToUtc` method is also broken (uses `SpecifyKind` instead of `ToUniversalTime` for Local dates), and the report defaults use `DateTime.Now` instead of `DateTime.UtcNow`, creating inconsistent date handling.

## Changes Required

### 1. `Controllers/StudentAttendanceController.cs`

**Fix ToUtc method (line 11-12):**
```csharp
private static DateTime ToUtc(DateTime date) =>
    date.Kind switch
    {
        DateTimeKind.Utc => date,
        DateTimeKind.Local => date.ToUniversalTime(),
        _ => DateTime.SpecifyKind(date, DateTimeKind.Utc)
    };
```

**Fix SaveAttendance POST handler to show validation errors:**
```csharp
public async Task<IActionResult> SaveAttendance(DailyAttendanceViewModel model)
{
    model.SelectedDate = ToUtc(model.SelectedDate);

    if (!ModelState.IsValid)
    {
        model.ClassList = await attendanceService.GetClassListAsync();
        var errors = string.Join("; ", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
        TempData["ErrorMessage"] = $"Validation failed: {errors}";
        return RedirectToAction(nameof(StudentAttendance), new { selectedDate = model.SelectedDate, selectedClassId = model.SelectedClassId });
    }
    
    int userId = 1;
    var success = await attendanceService.SaveAttendanceAsync(model, userId);
    
    if (success)
        TempData["SuccessMessage"] = "Attendance saved successfully!";
    else
        TempData["ErrorMessage"] = "Failed to save attendance. Please try again.";
    
    return RedirectToAction(nameof(StudentAttendance), new { selectedDate = model.SelectedDate, selectedClassId = model.SelectedClassId });
}
```

Key changes:
- Move `ToUtc(model.SelectedDate)` before `ModelState.IsValid` check (so date is always normalized).
- When invalid: collect errors, set TempData, redirect (instead of returning View directly).
- Remove comment about "adjust based on your auth system".

**Fix report defaults (lines 93-96):**
```csharp
if (!fromDate.HasValue)
    fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
if (!toDate.HasValue)
    toDate = DateTime.UtcNow.Date;
```

### 2. `Services/AttendanceService.cs`

**Fix ToUtc method (line 23-24):**
```csharp
private static DateTime ToUtc(DateTime date) =>
    date.Kind switch
    {
        DateTimeKind.Utc => date,
        DateTimeKind.Local => date.ToUniversalTime(),
        _ => DateTime.SpecifyKind(date, DateTimeKind.Utc)
    };
```

### 3. `Models/StudentAttendanceViewModel.cs`

**Fix SelectedClassId validation (line 17-20):**
```csharp
[Required(ErrorMessage = "Please select a class")]
[Range(1, int.MaxValue, ErrorMessage = "Please select a valid class")]
[Display(Name = "Class")]
public int SelectedClassId { get; set; }
```

### 4. `Views/StudentAttendance/StudentAttendance.cshtml`

**Add validation summary below the heading (after line 15):**
```html
<partial name="_ValidationSummary" />
```

Or add inline:
```html
@if (ViewData.ModelState.ErrorCount > 0)
{
    <div class="alert alert-danger">
        <ul>
            @foreach (var error in ViewData.ModelState.Values.SelectMany(v => v.Errors))
            {
                <li>@error.ErrorMessage</li>
            }
        </ul>
    </div>
}
```

### 5. `Entities/ClassEntity.cs` (optional)

**Line 49:** No change needed — `DateTime.Now.Year` is acceptable for academic year default.

## Testing
1. Run the app.
2. Go to Student Attendance, select a class and date, mark some students, click Save.
3. Verify green success message appears.
4. Go to Attendance Report, click Generate (no filters).
5. Verify students appear with correct attendance counts.
6. Reload the attendance page for same date/class — verify previously saved statuses are remembered.
