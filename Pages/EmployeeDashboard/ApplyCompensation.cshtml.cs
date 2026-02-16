using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.EmployeeDashboard.AcademicEmployee
{
    public class ApplyCompensationModel : PageModel
    {
        private readonly IAcademicRepository _repo;

        public ApplyCompensationModel(IAcademicRepository repo)
        {
            _repo = repo;
        }


        [BindProperty]
        [Required(ErrorMessage = "Compensation Date is required")]
        public DateTime CompensationDate { get; set; } = DateTime.Now;

        [BindProperty]
        [Required(ErrorMessage = "Original Work Date is required")]
        public DateTime OriginalDate { get; set; } = DateTime.Now;

        [BindProperty]
        [Required(ErrorMessage = "Please provide a reason")]
        public string Reason { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Replacement ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Employee ID")]
        public int ReplacementId { get; set; }


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

            if (ReplacementId == empId)
            {
                ModelState.AddModelError("ReplacementId", "You cannot replace yourself!");
            }

            if (!ModelState.IsValid) return Page();

            try
            {
                await _repo.SubmitCompensationLeave(empId, CompensationDate, Reason, OriginalDate, ReplacementId);

                TempData["Success"] = "Compensation Leave Submitted Successfully!";
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