using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.EmployeeDashboard.AcademicEmployee
{
    public class ApproveUnpaidModel : PageModel
    {
        private readonly IAcademicRepository _repo;

        public ApproveUnpaidModel(IAcademicRepository repo)
        {
            _repo = repo;
        }

        [BindProperty]
        [Required(ErrorMessage = "Request ID is required")]
        public int RequestId { get; set; }

        public IActionResult OnGet()
        {
            // Security Check
            if (Request.Cookies["EmployeeID"] == null)
            {
                return RedirectToPage("/Login");
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            string? cookieId = Request.Cookies["EmployeeID"];
            if (string.IsNullOrEmpty(cookieId)) return RedirectToPage("/Login");

            // This is the ID of the Dean/President approving the request
            int adminId = int.Parse(cookieId);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Call the SQL Procedure
                await _repo.ApproveUnpaidLeave(RequestId, adminId);

                TempData["Success"] = "Action Processed!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return Page();
            }
        }
    }
}