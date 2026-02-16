using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;
using University_HR_ManagementSystem.Models;

namespace University_HR_ManagementSystem.Pages.HRDashboard
{
    public class ManageUnpaidModel : PageModel
    {
        private readonly IConfiguration _config;

        public ManageUnpaidModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public int SelectedRequestID { get; set; }

        public List<SelectListItem> UnpaidLeaves { get; set; } = new();

        public string Message { get; set; }
        public string MessageCss { get; set; }

        public async Task OnGet()
        {
            var hrId = Request.Cookies["EmployeeID"];

            if (hrId == null)
            {
                Message = "HR session expired. Please log in again.";
                MessageCss = "alert-warning";
                return;
            }

            await LoadUnpaidRequests(hrId);
        }

        public async Task<IActionResult> OnPost()
        {
            var hrId = Request.Cookies["EmployeeID"];

            if (hrId == null)
            {
                Message = "HR session expired. Please log in again.";
                MessageCss = "alert-warning";
                return Page();
            }

            if (SelectedRequestID == 0)
            {
                Message = "Please select a request.";
                MessageCss = "alert-danger";
                await LoadUnpaidRequests(hrId);
                return Page();
            }

            try
            {
                using var conn = new SqlConnection(_config.GetConnectionString("University_HR_ManagementSystemContext"));
                await conn.OpenAsync();

                // Call the UNPAID ONLY stored procedure
                using var cmd = new SqlCommand("HR_approval_Unpaid", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@request_ID", SelectedRequestID);
                cmd.Parameters.AddWithValue("@HR_ID", Convert.ToInt32(hrId));

                await cmd.ExecuteNonQueryAsync();

                // Read the updated final approval status
                using var statusCmd = new SqlCommand(
                    "SELECT final_approval_status FROM [Leave] WHERE request_ID = @rid",
                    conn
                );
                statusCmd.Parameters.AddWithValue("@rid", SelectedRequestID);

                var status = (string?)await statusCmd.ExecuteScalarAsync();

                if (status == "Approved")
                {
                    Message = $"Request {SelectedRequestID} was APPROVED.";
                    MessageCss = "alert-success";
                }
                else if (status == "Rejected")
                {
                    Message = $"Request {SelectedRequestID} was REJECTED.";
                    MessageCss = "alert-danger";
                }
                else
                {
                    Message = "Request processed.";
                    MessageCss = "alert-info";
                }
            }
            catch (Exception ex)
            {
                Message = "Error processing request: " + ex.Message;
                MessageCss = "alert-danger";
            }

            await LoadUnpaidRequests(hrId); // reload list
            return Page();
        }

        private async Task LoadUnpaidRequests(string hrId)
        {
            UnpaidLeaves.Clear();

            using var conn = new SqlConnection(_config.GetConnectionString("University_HR_ManagementSystemContext"));
            await conn.OpenAsync();

            var query = @"
            SELECT L.request_ID,
                   CONCAT('Request ID: ', L.request_ID,' - Start Date: ', L.start_date,
                    ' - End Date: ', L.end_date, ' - NO. Days: ',L.num_days, 
                    ' - Employee ID: ', U.Emp_ID) AS DisplayText
            FROM Employee_Approve_Leave EAL
            JOIN [Leave] L ON L.request_ID = EAL.leave_ID
            JOIN Unpaid_Leave U ON U.request_ID = L.request_ID
            WHERE EAL.Emp1_ID = @HR_ID
              AND EAL.status = 'Pending'
              AND L.final_approval_status = 'Pending'
            ORDER BY L.date_of_request DESC;
            ";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@HR_ID", Convert.ToInt32(hrId));

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UnpaidLeaves.Add(new SelectListItem
                {
                    Value = reader["request_ID"].ToString(),
                    Text = reader["DisplayText"].ToString()
                });
            }
        }
    }
}