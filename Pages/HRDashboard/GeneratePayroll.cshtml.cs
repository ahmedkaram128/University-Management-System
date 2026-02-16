using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;
using University_HR_ManagementSystem.Models;

namespace University_HR_ManagementSystem.Pages.HRDashboard
{
    public class GeneratePayrollModel : PageModel
    {
        private readonly IConfiguration _config;
        public GeneratePayrollModel(IConfiguration config) => _config = config;

        // Inputs
        [BindProperty, Required] public int EmployeeID { get; set; }
        [BindProperty]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        public int Month { get; set; }

        [BindProperty]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        public int Year { get; set; }

        // Outputs
        public List<PayrollRecord> MonthlyPayrolls { get; set; } = new();
        [TempData] public string Message { get; set; }
        [TempData] public string MessageCss { get; set; }

        public async Task OnGetAsync()
        {
            MonthlyPayrolls = await GetPayrollsAsync(DateTime.Now.Month, DateTime.Now.Year);
        }

        public async Task<IActionResult> OnPostGenerateAsync()
        {
            if (!ModelState.IsValid)
            {
                MonthlyPayrolls = await GetPayrollsAsync(Month, Year);
                return Page();
            }

            using var conn = new SqlConnection(_config.GetConnectionString("University_HR_ManagementSystemContext"));
            await conn.OpenAsync();

            // Check employee exists
            var existsCmd = new SqlCommand("SELECT COUNT(*) FROM Employee WHERE employee_id = @id", conn);
            existsCmd.Parameters.AddWithValue("@id", EmployeeID);
            if ((int)await existsCmd.ExecuteScalarAsync() == 0)
            {
                Message = "Employee does not exist.";
                MessageCss = "alert-warning";
                MonthlyPayrolls = await GetPayrollsAsync(Month, Year);
                return Page();
            }

            // Calculate first and last day of month
            var firstDay = new DateTime(Year, Month, 1);
            var lastDay = new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month));

            // Execute payroll procedure
            try
            {
                var procCmd = new SqlCommand("Add_Payroll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                procCmd.Parameters.AddWithValue("@employee_ID", EmployeeID);
                procCmd.Parameters.AddWithValue("@from", firstDay);
                procCmd.Parameters.AddWithValue("@to", lastDay);

                await procCmd.ExecuteNonQueryAsync();
                Message = "Payroll generated successfully.";
                MessageCss = "alert-success";
            }
            catch (Exception ex)
            {
                Message = "Error executing payroll: " + ex.Message;
                MessageCss = "alert-danger";
            }

            // Load payrolls for this month
            MonthlyPayrolls = await GetPayrollsAsync(Month, Year);
            return Page();
        }

        private async Task<List<PayrollRecord>> GetPayrollsAsync(int month, int year)
        {
            var list = new List<PayrollRecord>();
            using var conn = new SqlConnection(_config.GetConnectionString("University_HR_ManagementSystemContext"));
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                SELECT * FROM Payroll
                WHERE MONTH(from_date) = @month AND YEAR(from_date) = @year
                ORDER BY ID DESC", conn);
            cmd.Parameters.AddWithValue("@month", month);
            cmd.Parameters.AddWithValue("@year", year);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new PayrollRecord
                {
                    PayrollID = reader.GetInt32(reader.GetOrdinal("ID")),
                    EmployeeID = reader.GetInt32(reader.GetOrdinal("emp_ID")),
                    PaymentDate = reader.GetDateTime(reader.GetOrdinal("payment_date")),
                    FinalSalaryAmount = reader.GetDecimal(reader.GetOrdinal("final_salary_amount")),
                    FromDate = reader.GetDateTime(reader.GetOrdinal("from_date")),
                    ToDate = reader.GetDateTime(reader.GetOrdinal("to_date")),
                    BonusAmount = reader.IsDBNull(reader.GetOrdinal("bonus_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("bonus_amount")),
                    DeductionsAmount = reader.IsDBNull(reader.GetOrdinal("deductions_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("deductions_amount")),
                    Comments = reader["comments"]?.ToString() ?? ""
                });
            }
            return list;
        }
    }
}
