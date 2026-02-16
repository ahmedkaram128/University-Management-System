using Microsoft.AspNetCore.Mvc.RazorPages;
using University_HR_ManagementSystem.Services;
using University_HR_ManagementSystem.Models;

public class RemoveResignedDeductionsModel : PageModel
{
    private readonly IAdminRepository _repo;

    public RemoveResignedDeductionsModel(IAdminRepository repo)
    {
        _repo = repo;
    }

    public List<ResignedDeductionVM> Deductions { get; set; } = new();
    public int RemovedCount { get; set; }
    public bool Executed { get; set; }

    public async Task OnGet()
    {
        Deductions = await _repo.GetResignedDeductionsAsync();
    }

    public async Task OnPost()
    {
        Deductions = await _repo.GetResignedDeductionsAsync();
        RemovedCount = await _repo.RemoveResignedDeductionsAsync();
        Executed = true;

        // Refresh the list after removal
        Deductions = await _repo.GetResignedDeductionsAsync();
    }
}
