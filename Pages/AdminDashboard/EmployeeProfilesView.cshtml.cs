using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Net.NetworkInformation;
using University_HR_ManagementSystem.Models;
using University_HR_ManagementSystem.Services;

public class EmployeeProfilesViewModel : PageModel
{
    private readonly IAdminRepository _repo;

    public EmployeeProfilesViewModel(IAdminRepository repo)
    {
        _repo = repo;
    }

    public List<EmployeeProfileViewVM> profileTable { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public async Task OnGet()
    {
        // Load employees once the page is opened
        try 
        {
            ErrorMessage = null;
            profileTable = await _repo.getAllEmployeeProfilesAsync();
        }
        catch (Exception ex) 
        {
            ErrorMessage = "Error loading data: " + ex.Message;
        }
    }
}
