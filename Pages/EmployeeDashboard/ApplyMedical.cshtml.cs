using System.ComponentModel.DataAnnotations; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.EmployeeDashboard.AcademicEmployee
{
    public class ApplyMedicalModel : PageModel
    {
        private readonly IAcademicRepository _repo;

        public ApplyMedicalModel(IAcademicRepository repo)
        {
            _repo = repo;
        }

        //  Data Binding (connects html inp. to csharp variables) & Validation Properties

        [BindProperty]
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [BindProperty]
        [Required(ErrorMessage = "End Date is required")]
        public DateTime EndDate { get; set; } = DateTime.Now;

        [BindProperty]
        [Required(ErrorMessage = "Please select a leave type")]
        public string Type { get; set; } = "sick";

        [BindProperty]
        public bool InsuranceStatus { get; set; }

        [BindProperty]
        public string? DisabilityDetails { get; set; } 

        [BindProperty]
        [Required(ErrorMessage = "Document Description is required")]
        [MinLength(5, ErrorMessage = "Description must be at least 5 characters")]
        public string DocumentDescription { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "File Name is required (e.g. report.pdf)")]
        public string FileName { get; set; }


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

            // Ensure Start Date is before End Date
            if (StartDate > EndDate)
            {
                //modelstate zy validation check 3ala el date
                ModelState.AddModelError("EndDate", "End Date must be after Start Date!");
            }

            // 3. Model State Check: If any field is missing or invalid, stop and return to the page
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // If validation passes, submit to the database
                await _repo.SubmitMedicalLeave(
                    empId,
                    StartDate,
                    EndDate,
                    Type,
                    InsuranceStatus,
                    DisabilityDetails,
                    DocumentDescription,
                    FileName
                );

                TempData["Success"] = "Medical Leave Submitted Successfully!";
                return RedirectToPage("/EmployeeDashboard/ApplyLeave");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error processing request: " + ex.Message;
                return Page();
            }
        }
    }
}