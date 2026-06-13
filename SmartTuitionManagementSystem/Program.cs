using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Data;
using SmartTuitionManagementSystem.Services;
using SmartTuitionManagementSystem.Services.Interface;
using SmartTuitionManagementSystem.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Set culture to Indian English for rupee currency formatting
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-IN");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-IN");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext for PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register all services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<ITeacherAttendanceService, TeacherAttendanceService>();
builder.Services.AddScoped<IFeeService, FeeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();