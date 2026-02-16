using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Update.Internal;
using University_HR_ManagementSystem.Models;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.Employee
{
public class ApproveLeaveModel : PageModel
{
    private readonly IEmployeeRepository _repo;

    public ApproveLeaveModel(IEmployeeRepository repo)
    {
        _repo = repo;
    }

    public string ActiveTab { get; set; } = "annual";

    public List<AnnualLeaveVM> AnnualRequests { get; set; } = new();
    public List<UnpaidLeaveVM> UnpaidRequests { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string type)
    {
        int employee_id = int.Parse(Request.Cookies["EmployeeID"]);
        ActiveTab = type ?? "annual";

        AnnualRequests = await _repo.GetAnnualLeaves(employee_id);
        UnpaidRequests = await _repo.GetUnpaidLeaves(employee_id);
        return Page();
    }

    public async Task<IActionResult> OnPostValidate(int RequestID, int replacement_emp, string LeaveType)
    {
        
        int employee_id = int.Parse(Request.Cookies["EmployeeID"]);

        bool flag = await _repo.ValidateLeave(RequestID, employee_id, replacement_emp, LeaveType);

        if (flag)
            TempData["SuccessMessage"] = 
                $"Request {RequestID} has been **Approved**.";
        else
        TempData["FailedMessage"] = 
            $"Request {RequestID} has been **Rejected**.";

        return RedirectToPage(new { type = LeaveType });
    }

}
}