using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.EmployeeDashboard.AcademicEmployee
{
    public class ApplyAnnualModel : PageModel
    {
        private readonly IAcademicRepository _repo;

        public ApplyAnnualModel(IAcademicRepository repo)
        {
            _repo = repo;
        }

        [BindProperty]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [BindProperty]
        public DateTime EndDate { get; set; } = DateTime.Now;

        [BindProperty]
        public int ReplacementId { get; set; }

        public IActionResult OnGet()
        {
            if (Request.Cookies["EmployeeID"] == null)
            {
                return RedirectToPage("/Login");
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            string cookieId = Request.Cookies["EmployeeID"];
            if (string.IsNullOrEmpty(cookieId)) return RedirectToPage("/Login");

            int empId = int.Parse(cookieId);

            try
            {

                await _repo.SubmitAnnualLeave(empId, StartDate, EndDate, ReplacementId);

                TempData["Success"] = "Annual Leave Submitted Successfully!";

                return RedirectToPage("/EmployeeDashboard/ApplyLeave");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return Page();
            }
        }
    }
}