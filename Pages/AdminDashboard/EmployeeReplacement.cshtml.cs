using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using University_HR_ManagementSystem.Models;
using University_HR_ManagementSystem.Services;
using System.ComponentModel.DataAnnotations;

namespace University_HR_ManagementSystem.Pages.AdminDashboard
{
    public class EmployeeReplacementModel : PageModel
    {
        private readonly IAdminRepository _repo;

        public EmployeeReplacementModel(IAdminRepository repo)
        {
            _repo = repo;
        }

        [BindProperty]
        [Required(ErrorMessage = "Please select the employee that will be replaced.")]
        [Range(1, int.MaxValue, ErrorMessage = "Employee ID must be a positive number.")]
        public int Emp1_ID { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please select the employee that will replace Emp1.")]
        [Range(1, int.MaxValue, ErrorMessage = "Employee ID must be a positive number.")]
        public int Emp2_ID { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please select the starting date.")]
        public DateTime FromDate { get; set; } = DateTime.Today;

        [BindProperty]
        [Required(ErrorMessage = "Please select the ending date.")] 
        public DateTime ToDate { get; set; } = DateTime.Today;

        public List<EmpReplacesEmpVM> Replacements { get; set; } = new();


        [TempData]
        public string? Message { get; set; }
        [TempData]
        public string? Error { get; set; }

        public async Task OnGetAsync()
        {
            Replacements = await _repo.GetAllReplacementsAsync();
        }

        public async Task<IActionResult> OnPostAddReplacementAsync()
        {
            if (!ModelState.IsValid)
            {
                Replacements = await _repo.GetAllReplacementsAsync();
                return Page();
            }
            try
            {
                bool rowsAffected = await _repo.AddReplacementAsync(Emp1_ID, Emp2_ID, FromDate, ToDate);
                if (rowsAffected)
                    Message = "Replacement added successfully!";
                else if(FromDate.CompareTo(ToDate) > 0)
                    Error = "Invalid Replacement! The 'From Date' cannot be after the 'To Date'";
                else 
                    Error = "Invalid Replacement!";
            }
            catch (SqlException ex)
            {
                Error = ex.Message; 
            }

            return RedirectToPage(); 
        }
    }
}
