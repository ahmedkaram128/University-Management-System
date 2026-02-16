using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;
using University_HR_ManagementSystem.Models;

namespace University_HR_ManagementSystem.Pages.HRDashboard
{
    public class MissingDaysModel : PageModel
    {
        private readonly IConfiguration _config;

        public MissingDaysModel(IConfiguration config)
        {
            _config = config;
        }

        // ------------------------- BOUND PROPERTY -----------------------------
        [BindProperty]
        [Required(ErrorMessage = "Employee ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Employee ID must be a positive number.")]
        public int EmployeeID { get; set; }

        // ---------------------------- PAGE DATA ---------------------------------
        public List<DeductionRecord> MonthlyDeductions { get; set; } = new();

        [TempData] public string? Message { get; set; }
        [TempData] public string? Error { get; set; }

        // ----------------------------- ON GET -----------------------------------
        public async Task OnGetAsync()
        {
            MonthlyDeductions = await GetCurrentMonthDeductionsAsync();
        }

        // ----------------------------- ON POST ----------------------------------
        public async Task<IActionResult> OnPostAddDeductionAsync()
        {
            if (!ModelState.IsValid)
            {
                MonthlyDeductions = await GetCurrentMonthDeductionsAsync();
                return Page();
            }

            using var conn = new SqlConnection(_config.GetConnectionString("University_HR_ManagementSystemContext"));
            await conn.OpenAsync();

            // 1. Check if employee exists
            var existsCmd = new SqlCommand("SELECT COUNT(*) FROM Employee WHERE employee_id = @id", conn);
            existsCmd.Parameters.AddWithValue("@id", EmployeeID);

            if ((int)await existsCmd.ExecuteScalarAsync() == 0)
            {
                ModelState.AddModelError("EmployeeID", "Employee does not exist in the database.");
                MonthlyDeductions = await GetCurrentMonthDeductionsAsync();
                return Page();
            }

            // 2. Execute deduction procedure
            try
            {
                var procCmd = new SqlCommand("Deduction_days", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                procCmd.Parameters.AddWithValue("@employee_ID", EmployeeID);
                await procCmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Error = "Error executing procedure: " + ex.Message;
                return RedirectToPage();
            }

            // 3. Fetch the new deduction
            var fetchCmd = new SqlCommand(@"
                SELECT TOP 1 *
                FROM Deduction
                WHERE emp_ID = @id AND type = 'missing_days' AND date = CAST(GETDATE() AS DATE)
                ORDER BY deduction_ID DESC;", conn);
            fetchCmd.Parameters.AddWithValue("@id", EmployeeID);

            DeductionRecord? newDeduction = null;

            using var reader = await fetchCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                newDeduction = new DeductionRecord
                {
                    DeductionID = reader.GetInt32(reader.GetOrdinal("deduction_ID")),
                    EmployeeID = reader.GetInt32(reader.GetOrdinal("emp_ID")),
                    Date = reader.GetDateTime(reader.GetOrdinal("date")),
                    Amount = reader.GetDecimal(reader.GetOrdinal("amount")),
                    Type = reader.GetString(reader.GetOrdinal("type")),
                    Status = reader.GetString(reader.GetOrdinal("status")),
                };
            }

            if (newDeduction == null)
            {
                ModelState.AddModelError("EmployeeID", "No deduction was created — employee has no missing days.");
                MonthlyDeductions = await GetCurrentMonthDeductionsAsync();
                return Page();
            }

            TempData["SuccessMessage"] = "Deduction calculated successfully.";
            return RedirectToPage();
        }

        // ------------------------------------------------------------------------
        // HELPER: Load current month deductions
        // ------------------------------------------------------------------------
        private async Task<List<DeductionRecord>> GetCurrentMonthDeductionsAsync()
        {
            var list = new List<DeductionRecord>();

            using var conn = new SqlConnection(_config.GetConnectionString("University_HR_ManagementSystemContext"));
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                SELECT deduction_ID, emp_ID, date, amount, type, status
                FROM Deduction
                WHERE 
                    type = 'missing_days'
                    AND date >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                    AND date <= CAST(GETDATE() AS DATE)
                ORDER BY deduction_ID DESC;", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new DeductionRecord
                {
                    DeductionID = reader.GetInt32(0),
                    EmployeeID = reader.GetInt32(1),
                    Date = reader.GetDateTime(2),
                    Amount = reader.GetDecimal(3),
                    Type = reader.GetString(4),
                    Status = reader.GetString(5)
                });
            }

            return list;
        }
    }
}
