using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Models;
using University_HR_ManagementSystem.Pages.AdminDashboard;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.AdminDashboard
{
    public class InitializeAttendanceModel : PageModel
    {
        private readonly IAdminRepository _repo;

        public InitializeAttendanceModel(IAdminRepository repo)
        {
            _repo = repo;
        }

        public List<AttendanceVM> todayAttendance { get; set; } = new();

        [TempData]
        public string? Message { get; set; }

        public async Task OnGet()
        {
            todayAttendance = await _repo.GetAttendanceByDateAsync(DateTime.Today);
        }

        public async Task<IActionResult> OnPostInitiateAsync()
        {
            int rowsAffected = await _repo.InitiateAttendanceAsync();

            Message = rowsAffected+" Attendance record(s) were added.";

            return RedirectToPage();  
        }
    }
}



