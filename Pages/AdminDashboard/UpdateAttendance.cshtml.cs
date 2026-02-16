using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Models;
using University_HR_ManagementSystem.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace University_HR_ManagementSystem.Pages.AdminDashboard
{
    public class UpdateAttendanceModel : PageModel
    {
        private readonly IAdminRepository _repo;

        public UpdateAttendanceModel(IAdminRepository repo)
        {
            _repo = repo;
        }

        [BindProperty]
        public int EmployeeId { get; set; }

        [BindProperty]
        public TimeSpan? CheckIn { get; set; }

        [BindProperty]
        public TimeSpan? CheckOut { get; set; }

        public List<AttendanceVM> todayAttendance { get; set; } = new();

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGet()
        {
            todayAttendance = await _repo.GetAttendanceByDateAsync(DateTime.Today);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await _repo.UpdateAttendanceAsync(EmployeeId, CheckIn, CheckOut);
            bool updated = await _repo.UpdateAttendanceAsync(EmployeeId, CheckIn, CheckOut);
            if (!updated)
            {
                ErrorMessage = "There is no attendence record to update";
                EmployeeId = 0;
                CheckIn = null;
                CheckOut = null;

                ModelState.Clear();
                return RedirectToPage();
            }

            SuccessMessage = "The Attendence was succesfully updated";

            EmployeeId = 0; 
            CheckIn = null;
            CheckOut = null;

            ModelState.Clear();

            return RedirectToPage();

        }
    }
}