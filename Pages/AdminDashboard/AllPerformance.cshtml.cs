using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Services;
using University_HR_ManagementSystem.Models;

namespace University_HR_ManagementSystem.Pages.AdminDashboard
{
    public class AllPerformanceModel : PageModel
    {
        private readonly IAdminRepository _repo;
        public List<allPerformanceVM> performances { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public AllPerformanceModel(IAdminRepository repo)
        {
            _repo = repo;
        }

        
        public async Task OnGet()
        {
            try
            {
                ErrorMessage = null;
                performances = await _repo.getAllPerformanceAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error loading attendance: " + ex.Message;
            }
        }
    }
}
