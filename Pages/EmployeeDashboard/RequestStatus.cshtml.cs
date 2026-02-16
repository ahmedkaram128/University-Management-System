using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Models;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.Employee
{
    public class RequestStatusModel : PageModel
    {

        private readonly IEmployeeRepository _repo;
        public List<RequestStatusVM> Requests { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public RequestStatusModel(IEmployeeRepository repo)
        {
            _repo = repo;
        }
        public async Task OnGet()
        {
            try
            {
                int employee_id = int.Parse(Request.Cookies["EmployeeID"]);
                Requests = await _repo.getRequestStatus(employee_id);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error loading attendance: " + ex.Message;
            }
        }
    }
}
