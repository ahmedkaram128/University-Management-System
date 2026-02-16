using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using University_HR_ManagementSystem.Models;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.Employee
{
    public class DeductionsAttendance : PageModel
    {
        private readonly IEmployeeRepository _repo;

        public DeductionsAttendance(IEmployeeRepository repo) {
            _repo = repo;
        }


        public List<DeductionAttendanceVM> Deductions { get; set; } = new();
        [BindProperty] public int Month { get; set; }
        public List<SelectListItem> Months { get; set; }

        public string ErrorMessage { get; set; } = "";
        public async Task OnGetAsync()
        {   
            Months = new List<SelectListItem>
            {
                new SelectListItem("January", "1"),
                new SelectListItem("February", "2"),
                new SelectListItem("March", "3"),
                new SelectListItem("April", "4"),
                new SelectListItem("May", "5"),
                new SelectListItem("June", "6"),
                new SelectListItem("July", "7"),
                new SelectListItem("August", "8"),
                new SelectListItem("September", "9"),
                new SelectListItem("October", "10"),
                new SelectListItem("November", "11"),
                new SelectListItem("December", "12")
            };

            int employeeId = int.Parse(Request.Cookies["EmployeeID"]);
            Deductions = await _repo.GetDeductionsAttendance(employeeId, Month);
        }        

        public async Task<IActionResult> OnPostAsync()
        {
            Months = new List<SelectListItem>
            {
                new SelectListItem("January", "1"),
                new SelectListItem("February", "2"),
                new SelectListItem("March", "3"),
                new SelectListItem("April", "4"),
                new SelectListItem("May", "5"),
                new SelectListItem("June", "6"),
                new SelectListItem("July", "7"),
                new SelectListItem("August", "8"),
                new SelectListItem("September", "9"),
                new SelectListItem("October", "10"),
                new SelectListItem("November", "11"),
                new SelectListItem("December", "12")
            };

            int employeeId = int.Parse(Request.Cookies["EmployeeID"]);
            Deductions = await _repo.GetDeductionsAttendance(employeeId, Month);
            return Page();
        }
    }
}