using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.EmployeeDashboard.AcademicEmployee
{
    public class ApplyUnpaidModel : PageModel
    {
        private readonly IAcademicRepository _repo;

        public ApplyUnpaidModel(IAcademicRepository repo)
        {
            _repo = repo;
        }

        [BindProperty]
        [Required]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [BindProperty]
        [Required]
        public DateTime EndDate { get; set; } = DateTime.Now;

        [BindProperty]
        [Required]
       
        public string DocumentDescription { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        public string FileName { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            if (Request.Cookies["EmployeeID"] == null) return RedirectToPage("/Login");
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            string? cookieId = Request.Cookies["EmployeeID"];
            if (string.IsNullOrEmpty(cookieId)) return RedirectToPage("/Login");

            int empId = int.Parse(cookieId);

            if (StartDate > EndDate)
            {
                ModelState.AddModelError("EndDate", "End Date must be after Start Date!");
            }

            if (!ModelState.IsValid) return Page();

            try
            {
                await _repo.SubmitUnpaidLeave(empId, StartDate, EndDate, DocumentDescription, FileName);
                TempData["Success"] = "Unpaid Leave Submitted Successfully!";
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