using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace University_HR_ManagementSystem.Pages
{
    [AllowAnonymous]
    public class AccessDeniedModel : PageModel
    {
        public string ReturnUrl { get; set; } = "/Index";

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true){
                if (User.IsInRole("Admin"))
                    ReturnUrl = "/AdminDashboard/AdminDashboard";
                else if (User.IsInRole("HR"))
                    ReturnUrl = "/HRDashboard/HRDashboard";
                else if (User.IsInRole("Employee"))
                    ReturnUrl = "/EmployeeDashboard/EmployeeDashboard";
                else
                    ReturnUrl = "/Index";
            }
            else ReturnUrl = "/Index";
            
        }
    }
}
