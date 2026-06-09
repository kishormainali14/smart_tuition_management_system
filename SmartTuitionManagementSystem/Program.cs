using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Data;
using SmartTuitionManagementSystem.Services;
using SmartTuitionManagementSystem.Services.Interface;

//using SmartTuitionManagementSystem.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var webApplicationBuilder = builder;
webApplicationBuilder.Services.AddControllersWithViews();  

// Add DbContext for PostgreSQL
webApplicationBuilder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection")));

// Add Session services
webApplicationBuilder.Services.AddDistributedMemoryCache();
webApplicationBuilder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpContextAccessor
webApplicationBuilder.Services.AddHttpContextAccessor();

// Register User Service
webApplicationBuilder.Services.AddScoped<IUserService, UserService>();
// Add this line with other service registrations
webApplicationBuilder.Services.AddScoped<IStudentService, StudentService>();
webApplicationBuilder.Services.AddScoped<ITeacherService, TeacherService>();
webApplicationBuilder.Services.AddScoped<IAttendanceService, AttendanceService>();
webApplicationBuilder.Services.AddScoped<IClassService, ClassService>();
var app = webApplicationBuilder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Use Session - this handles your authentication
app.UseSession();

// No app.UseAuthentication() needed

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();