using Microsoft.Data.SqlClient;
using System.Data;

namespace University_HR_ManagementSystem.Services
{
    public class AcademicRepository : IAcademicRepository
    {
        private readonly string _connectionString;

        public AcademicRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("University_HR_ManagementSystemContext");
        }

        public async Task<bool> SubmitAccidentalLeave(int empId, DateTime start, DateTime end)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {

                SqlCommand cmd = new SqlCommand("Submit_accidental", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@employee_ID", empId);
                cmd.Parameters.AddWithValue("@start_date", start);
                cmd.Parameters.AddWithValue("@end_date", end);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
        }
        public async Task<bool> SubmitMedicalLeave(int empId, DateTime start, DateTime end, string type, bool insurance, string disability, string docDesc, string fileName)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Submit_medical", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@employee_ID", empId);
                cmd.Parameters.AddWithValue("@start_date", start);
                cmd.Parameters.AddWithValue("@end_date", end);
                cmd.Parameters.AddWithValue("@medical_type", type);
                cmd.Parameters.AddWithValue("@insurance_status", insurance);
                cmd.Parameters.AddWithValue("@disability_details", disability ?? (object)DBNull.Value); // Handle Null
                cmd.Parameters.AddWithValue("@document_description", docDesc);
                cmd.Parameters.AddWithValue("@file_name", fileName);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
        }

        public async Task<bool> SubmitUnpaidLeave(int empId, DateTime start, DateTime end, string docDesc, string fileName)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Submit_unpaid", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@employee_ID", empId);
                cmd.Parameters.AddWithValue("@start_date", start);
                cmd.Parameters.AddWithValue("@end_date", end);
                cmd.Parameters.AddWithValue("@document_description", docDesc);
                cmd.Parameters.AddWithValue("@file_name", fileName);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
        }

        public async Task<bool> SubmitCompensationLeave(int empId, DateTime compensationDate, string reason, DateTime originalDay, int replacementId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Submit_compensation", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@employee_ID", empId);
                cmd.Parameters.AddWithValue("@compensation_date", compensationDate);
                cmd.Parameters.AddWithValue("@reason", reason);
                cmd.Parameters.AddWithValue("@date_of_original_workday", originalDay);
                cmd.Parameters.AddWithValue("@rep_emp_id", replacementId);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
        }
        // 5. Approve Unpaid Leave
        public async Task<bool> ApproveUnpaidLeave(int requestId, int adminId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Upperboard_approve_unpaids", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@request_ID", requestId);
                cmd.Parameters.AddWithValue("@upperboard_ID", adminId);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
        }

        // 6. Approve Annual Leave
        public async Task<bool> ApproveAnnualLeave(int requestId, int adminId, int replacementId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                
                SqlCommand cmd = new SqlCommand("Upperboard_approve_annual", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@request_ID", requestId);
                cmd.Parameters.AddWithValue("@Upperboard_ID", adminId);
                cmd.Parameters.AddWithValue("@replacement_ID", replacementId);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
        }

        // 7. Evaluate Employee
        public async Task<bool> EvaluateEmployee(int empId, int rating, string comment, string semester)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
             
                SqlCommand cmd = new SqlCommand("Dean_andHR_Evaluation", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@employee_ID", empId);
                cmd.Parameters.AddWithValue("@rating", rating);
                cmd.Parameters.AddWithValue("@comment", comment);
                cmd.Parameters.AddWithValue("@semester", semester);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
        }
        public async Task<bool> SubmitAnnualLeave(int empId, DateTime start, DateTime end, int replacementId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Submit_annual", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@employee_ID", empId);
                cmd.Parameters.AddWithValue("@start_date", start);
                cmd.Parameters.AddWithValue("@end_date", end);
                cmd.Parameters.AddWithValue("@replacement_emp", replacementId);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
        }

        public async Task<bool> ApproveUnpaid(int requestId, int adminId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Upperboard_approve_unpaids", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@request_ID", requestId);
                cmd.Parameters.AddWithValue("@upperboard_ID", adminId);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
        }

        public async Task<bool> isDean(int empId)
        {       
            const string sql = @"
                select employee_id from Employee e 
                where exists (
                    select * from Employee_Role er where
                    er.emp_ID=e.employee_id and er.role_name='Dean'
                )
                and employee_id=@EmployeeID
            ";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@EmployeeID", empId);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (reader.HasRows) return true;
            return false;
        }

        public async Task<bool> getValidity(int dean, int emp)
        {
            const string sql = @"
                select d.employee_id from Employee d 
                where exists (
                    select * from Employee e where e.dept_name=d.dept_name and e.employee_id=@EmployeeID
                ) and d.employee_id=@DeanID            
            ";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@EmployeeID", emp);
            cmd.Parameters.AddWithValue("@DeanID", dean);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (reader.HasRows) return true;
            return false;

        }
    }
}