using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Services;
using System.ComponentModel.DataAnnotations;

namespace University_HR_ManagementSystem.Pages.EmployeeDashboard.AcademicEmployee
{
    public class EvaluateEmployeeModel : PageModel
    {
        private readonly IAcademicRepository _repo;
        public EvaluateEmployeeModel(IAcademicRepository repo) { _repo = repo; }

        [BindProperty, Required] public int TargetEmployeeId { get; set; }
        [BindProperty, Required, Range(1, 5)] public int Rating { get; set; }
        [BindProperty, Required] public string Semester { get; set; }
        [BindProperty, Required] public string Comment { get; set; }

        public IActionResult OnGet()
        {
            if (Request.Cookies["EmployeeID"] == null) return RedirectToPage("/Index");
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            string? cookieId = Request.Cookies["EmployeeID"];
            if (string.IsNullOrEmpty(cookieId)) return RedirectToPage("/Index");
            int loggedInId = int.Parse(cookieId);

            if (TargetEmployeeId == loggedInId)
            {
                TempData["Error"] = "You cannot evaluate yourself!";
                return Page();
            }

            if (! await _repo.isDean(loggedInId))
            {
                TempData["Error"] = "Only Deans can evaluate other Employees";
                return Page();
            }
            if (! await _repo.getValidity(loggedInId, TargetEmployeeId))
            {
                TempData["Error"] = "You can only evaluate Employees in the same Departement";
                return Page();
            }


            if (!ModelState.IsValid) return Page();

            try
            {
                await _repo.EvaluateEmployee(TargetEmployeeId, Rating, Comment, Semester);
                TempData["Success"] = "Evaluation Submitted Successfully!";

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


// SIX SEVENNNNN