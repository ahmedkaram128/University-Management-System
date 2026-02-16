using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

public class AdminDashboardModel : PageModel
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
