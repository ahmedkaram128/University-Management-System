using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using University_HR_ManagementSystem.Models;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.Employee
{
    public class LastMonthPayrollModel : PageModel
    {
        private readonly IEmployeeRepository _repo;

        public LastMonthPayrollModel(IEmployeeRepository repo) {
            _repo = repo;
        }

        public PayrollVM Payroll { get; set; } = new();

        public string ErrorMessage { get; set; } = "";

        public async Task OnGetAsync(){
            int employeeId = int.Parse(Request.Cookies["EmployeeID"]);
            Payroll = await _repo.GetEmployeePayroll(employeeId) ?? new PayrollVM(); 
        }

        public async Task<IActionResult> OnPostAsync()
        {   
            return Page();
        }
    }
}