using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Models;
using University_HR_ManagementSystem.Services;

namespace University_HR_ManagementSystem.Pages.AdminDashboard
{
    public class RejectedMedicalModel : PageModel
    {

        private readonly IAdminRepository _repo;
        public List<allRejectedMedicalsVM> rejectedMedicals { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public RejectedMedicalModel(IAdminRepository repo)
        {
            _repo = repo;
        }
        public async Task OnGet()
        {
            try
            {
                rejectedMedicals = await _repo.getRejectedMedicalAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error loading attendance: " + ex.Message;
            }
        }
    }
}
