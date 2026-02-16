using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Models;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.AdminDashboard
{
    public class AddHolidayModel : PageModel
    {
        private readonly IAdminRepository _repo;

        public AddHolidayModel(IAdminRepository repo)
        {
            _repo = repo;
        }

        [BindProperty]
        public AddHolidayVM Holiday { get; set; } = new AddHolidayVM();

        public bool Executed { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        // Stored Message
        public string? SuccessHolidayName { get; set; }
        public DateTime? SuccessStartDate { get; set; }
        public DateTime? SuccessEndDate { get; set; }

        public void OnGet()
        {
            Holiday.FromDate = DateTime.Today;
            Holiday.ToDate = DateTime.Today;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Executed = true;

            if (Holiday.FromDate > Holiday.ToDate)
            {
                ErrorMessage = "Start Date cannot be after End Date.";
                Success = false;
                return Page();
            }

            var result = await _repo.AddHolidayAsync(
                Holiday.HolidayName,
                Holiday.FromDate,
                Holiday.ToDate
            );

            if (result == "UPDATED")
            {
                Success = true;
                SuccessHolidayName = Holiday.HolidayName;
                SuccessStartDate = Holiday.FromDate;
                SuccessEndDate = Holiday.ToDate;

                ViewData["Updated"] = true;

                ModelState.Clear();
                Holiday = new AddHolidayVM
                {
                    FromDate = DateTime.Today,
                    ToDate = DateTime.Today
                };

                return Page();
            }

            if (result != "SUCCESS")
            {
                ErrorMessage = result;
                Success = false;
                return Page();
            }

            Success = true;
            SuccessHolidayName = Holiday.HolidayName;
            SuccessStartDate = Holiday.FromDate;
            SuccessEndDate = Holiday.ToDate;

            ViewData["Updated"] = null;   // PREVENT accidental update detection

            ModelState.Clear();
            Holiday = new AddHolidayVM
            {
                FromDate = DateTime.Today,
                ToDate = DateTime.Today
            };

            return Page();
        }

    }
}
