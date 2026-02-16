using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.AdminDashboard
{
    public class UpdateEmploymentStatusModel : PageModel
    {
        private readonly IAdminRepository _repo;

        public UpdateEmploymentStatusModel(IAdminRepository repo)
        {
            _repo = repo;
        }

        [BindProperty(SupportsGet = true)]
        public int EmployeeId { get; set; }

        public bool Executed { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }

        public async Task OnGet()
        {
            bool userSearched = Request.Query.ContainsKey("employeeId");

            if (!userSearched)
                return;

            Executed = true;

          
            if (EmployeeId < 1)
            {
                Success = false;
                Message = "Employee ID must be greater than 0";  
                return;
            }

            var result = await _repo.UpdateEmploymentStatusAsync(EmployeeId);

            
            string oldS = FormatStatus(result.OldStatus);
            string newS = FormatStatus(result.NewStatus);

            switch (result.Result)
            {
                case "NOT_FOUND":
                    Success = false;
                    Message = "Employee not found";   
                    break;

                case "NO_CHANGE":
                    Success = false;
                    Message = $"Employment status is currently up to date (<b>{oldS}</b>)"; 
                    break;

                case "UPDATED":
                    Success = true;
                    Message = $"Employment status has been updated from <b>{oldS}</b> to <b>{newS}</b>";  
                    break;

                default:
                    Success = false;
                    Message = "<b>Unknown error.</b>";   
                    break;
            }
        }

        private string FormatStatus(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return raw ?? "";

            return raw.ToLower() switch
            {
                "active" => "Active",
                "onleave" => "On-Leave",
                "notice_period" => "Notice Period",
                "resigned" => "Resigned",
                _ => raw
            };
        }
    }
}
