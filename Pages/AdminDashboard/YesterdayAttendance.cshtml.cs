using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Services;
using University_HR_ManagementSystem.Models;

namespace University_HR_ManagementSystem.Pages.AdminDashboard
{
    public class YesterdayAttendanceModel : PageModel
    {
        private readonly IAdminRepository _repo;

        public YesterdayAttendanceModel(IAdminRepository repo)
        {
            _repo = repo;
        }

        public List<AttendanceVM> Records { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task OnGet()
        {
            try
            {
                Records = await _repo.GetYesterdayAttendanceAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error loading attendance: " + ex.Message;
            }
        }
    }
}
