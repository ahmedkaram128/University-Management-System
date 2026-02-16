using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages
{
    public class LoginHRModel : PageModel
    {
        private readonly ILoginRepository loginRepository;
        public LoginHRModel(ILoginRepository _loginRepository)  {
            loginRepository = _loginRepository;
        }

        [BindProperty] public string ID { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";
        [BindProperty] public string URL { get; set; } = "/";

        public async Task<IActionResult> OnPost(){
            if (await loginRepository.validateHRLogin(ID, Password)){                
                Response.Cookies.Append("EmployeeID", ID, new CookieOptions());
                var claims = new List<Claim>{
                    new Claim(ClaimTypes.NameIdentifier, ID),
                    new Claim(ClaimTypes.Role, "HR")
                };
                await SignInUser(claims);
                return LocalRedirect("/HRDashboard/HRDashboard");
            }
            ModelState.AddModelError(string.Empty, "Invalid login credentials");
            return Page();
        }

        private async Task SignInUser(List<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );
        }

    }
}