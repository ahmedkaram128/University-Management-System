using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Data;
using University_HR_ManagementSystem.Models;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.AdminDashboard
{
    public class RemoveApprovedLeavesModel : PageModel
    {
        private readonly IAdminRepository _repo;
        private readonly University_HR_ManagementSystemContext _context;

        public RemoveApprovedLeavesModel(IAdminRepository repo, University_HR_ManagementSystemContext ctx)
        {
            _repo = repo;
            _context = ctx;
        }

        public List<RemoveApprovedLeaveVM> Records { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int EmployeeId { get; set; }

        public string? EmployeeName { get; set; }
        public bool RecordsLoaded { get; set; }

        public int RemovedCount { get; set; }
        public bool Executed { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGet()
        {
            if (Request.Query.ContainsKey("employeeId"))
            {
                if (EmployeeId <= 0)
                {
                    ErrorMessage = "Employee ID must be greater than 0.";
                    RecordsLoaded = false;
                    return;
                }

                var emp = _context.Employee.FirstOrDefault(e => e.EmployeeId == EmployeeId);

                if (emp == null)
                {
                    ErrorMessage = "Employee not found.";
                    RecordsLoaded = false;
                    return;
                }

                EmployeeName = $"{emp.FirstName} {emp.LastName}";
                Records = await _repo.GetApprovedLeaveAttendanceAsync(EmployeeId);
                RecordsLoaded = true;
            }
        }


        public async Task OnPost(int employeeId)
        {
            EmployeeId = employeeId;

            var emp = _context.Employee.FirstOrDefault(e => e.EmployeeId == EmployeeId);

            if (emp == null)
            {
                ErrorMessage = "Employee not found.";
                RecordsLoaded = false;
                return;
            }

            EmployeeName = $"{emp.FirstName} {emp.LastName}";

            // Count before deleting
            RemovedCount = await _repo.CountApprovedLeaveAttendanceAsync(EmployeeId);

            await _repo.RemoveApprovedLeavesAsync(EmployeeId);
            Executed = true;

            // Reload attendance (should be empty)
            Records = await _repo.GetApprovedLeaveAttendanceAsync(EmployeeId);
            RecordsLoaded = true;
        }
    }
}
