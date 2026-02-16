using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Models;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.AdminDashboard
{
    public class RemoveDayOffModel : PageModel
    {
        private readonly IAdminRepository _repo;

        public List<AttendanceVM> removedList = new();

        [BindProperty]
        public int EmployeeId { get; set; }

        [TempData]
        public string? Message { get; set; }

        [TempData]
        public string? Warning { get; set; }

        public RemoveDayOffModel(IAdminRepository repo)
        {
            _repo = repo;
        }

        public async void OnGetAsync()
        {
            Message = null;
            Warning = null;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            removedList = await _repo.RemoveDayOffAsync(EmployeeId);

            if(removedList.Count > 0) 
            {
                Message = removedList.Count + " attendance record(s) were removed.";
            }
            else
            {
                Warning = "No attendence records were removed.";
            }
            return Page(); 
        }
    }
}