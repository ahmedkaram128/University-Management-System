using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace University_HR_ManagementSystem.Pages.EmployeeDashboard
{
    public class EmployeeDashboardModel : PageModel
    {
        public void OnGet(){
        }

        public async Task<IActionResult> OnPostLogout(){
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            foreach (var cookie in Request.Cookies.Keys)
                Response.Cookies.Delete(cookie);

            return RedirectToPage("/Index");
        }
    }
}
