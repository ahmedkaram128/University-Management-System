using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Services;
using University_HR_ManagementSystem.Models;

public class EmployeesPerDeptModel : PageModel
{
    private readonly IAdminRepository _repo;

    public EmployeesPerDeptModel(IAdminRepository repo)
    {
        _repo = repo;
    }

    public List<EmployeesPerDeptVM> Departments { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public async Task OnGet()
    {
        try
        {
            Departments = await _repo.GetEmployeesPerDepartmentAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error loading data: " + ex.Message;
        }
    }
}
