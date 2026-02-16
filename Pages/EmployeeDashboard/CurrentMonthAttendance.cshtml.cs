using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using University_HR_ManagementSystem.Models;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.Employee
{
    public class CurrentMonthAttendanceModel : PageModel
    {
        private readonly IEmployeeRepository _repo;

        public CurrentMonthAttendanceModel(IEmployeeRepository repo) {
            _repo = repo;
        }


        public List<EmployeeAttendanceVM> Attendances { get; set; } = new();

        public string ErrorMessage { get; set; } = "";
        public async Task OnGetAsync() {

            if (Request.Cookies.ContainsKey("EmployeeID") && 
                int.TryParse(Request.Cookies["EmployeeID"], out int employeeId))
                Attendances = await _repo.GetAttendancesAsync(employeeId);
            
            else  ErrorMessage = "Could not retrieve valid Employee ID.";
        }        
        public async Task OnPostAsync(){
            int employeeId = int.Parse(Request.Cookies["EmployeeID"]);
            Attendances = await _repo.GetAttendancesAsync(employeeId);
        }
    }
}