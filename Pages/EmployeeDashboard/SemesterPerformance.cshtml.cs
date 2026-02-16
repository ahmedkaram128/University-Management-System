using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using University_HR_ManagementSystem.Models;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.Employee
{
    public class SemesterPerformanceModel : PageModel
    {
        private readonly IEmployeeRepository _repo;

        public SemesterPerformanceModel(IEmployeeRepository repo) {
            _repo = repo;
        }

        public List<SelectListItem> Semesters { get; set; } = new();

        [BindProperty] public string SelectedSemester { get; set; } = "";

        public List<SemesterPerformanceVM> Performances { get; set; } = new();

        public string ErrorMessage { get; set; } = "";

        public async Task OnGetAsync() {
            int employeeId = int.Parse(Request.Cookies["EmployeeID"]);
            var semesters = await _repo.GetEmployeeSemestersAsync(employeeId);

            Semesters = semesters
                .Select(s => new SelectListItem { Value = s, Text = s })
                .ToList();
        }

        public async Task OnPostAsync(){
            int employeeId = int.Parse(Request.Cookies["EmployeeID"]);
            var semesters = await _repo.GetEmployeeSemestersAsync(employeeId);
            Semesters = semesters
                .Select(s => new SelectListItem { Value = s, Text = s })
                .ToList();

            if (string.IsNullOrEmpty(SelectedSemester))
            {
                ErrorMessage = "Please select a semester.";
                return;
            }

            Performances = await _repo.GetSemesterPerformanceAsync(employeeId, SelectedSemester);
        }
    }
}