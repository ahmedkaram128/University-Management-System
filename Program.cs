using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using University_HR_ManagementSystem.Data;
using University_HR_ManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<University_HR_ManagementSystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("University_HR_ManagementSystemContext")
    ?? throw new InvalidOperationException("Connection string not found.")));

builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IAcademicRepository, AcademicRepository>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/AdminDashboard", "Admin");
    options.Conventions.AuthorizeFolder("/HRDashboard", "HR");
    options.Conventions.AuthorizeFolder("/EmployeeDashboard", "Employee");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/AccessDenied";  
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); 
        options.SlidingExpiration = true; 
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("HR", policy => policy.RequireRole("HR"));
    options.AddPolicy("Employee", policy => policy.RequireRole("Employee"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
