using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Services;
using University_HR_ManagementSystem.Models;

namespace University_HR_ManagementSystem.Pages.AdminDashboard
{
    public class RemoveHolidayAttendanceModel : PageModel
    {
        private readonly IAdminRepository _repo;

        public RemoveHolidayAttendanceModel(IAdminRepository repo)
        {
            _repo = repo;
        }

        public List<HolidayAttendanceVM> Records { get; set; } = new();
        public int RemovedCount { get; set; }
        public bool Executed { get; set; }

        public async Task OnGet()
        {
            Records = await _repo.GetHolidayAttendanceAsync();
        }

        public async Task OnPost()
        {
            Records = await _repo.GetHolidayAttendanceAsync();
            RemovedCount = await _repo.RemoveHolidayAttendanceAsync();
            Executed = true;

            // Reload list after deletion
            Records = await _repo.GetHolidayAttendanceAsync();
        }
    }
}
