using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.EmployeeDashboard
{
    public class ApplyAccidentalModel : PageModel
    {
        private readonly IAcademicRepository _repo;

        public ApplyAccidentalModel(IAcademicRepository repo)
        {
            _repo = repo;
        }

        [BindProperty]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [BindProperty]
        public DateTime EndDate { get; set; } = DateTime.Now;
        public DateTime LeaveDate { get; set; } = DateTime.Now;

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
                await _repo.SubmitAccidentalLeave(empId, LeaveDate, LeaveDate);

                TempData["Success"] = "Accidental Leave Submitted Successfully!";

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