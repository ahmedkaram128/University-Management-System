using System;
using System.Data;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace University_HR_ManagementSystem.Services
{
    public class LoginRepository : ILoginRepository
    {
        private readonly string connectionString;


        public LoginRepository(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection")
                ?? config.GetConnectionString("University_HR_ManagementSystemContext")
                ?? throw new InvalidOperationException("No connection string 'DefaultConnection' or 'University_HR_ManagementSystemContext' was found in configuration.");
        }

        public async Task<bool> validateHRLogin(string ID, string password) {
            if (!int.TryParse(ID, out int employeeId)) 
                return false; 

            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            const string validateSql = @"SELECT dbo.HRLoginValidation(@employee_id, @password)";

            await using var validateCmd = new SqlCommand(validateSql, conn);
            validateCmd.CommandType = CommandType.Text;
            validateCmd.Parameters.AddWithValue("@employee_id", employeeId); 
            validateCmd.Parameters.AddWithValue("@password", password);

            var result = await validateCmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
                return false;

            return Convert.ToInt32(result) == 1;
        }
    
        public async Task<bool> validateEmpLogin(string ID, string password){
            if (!int.TryParse(ID, out int employeeId)) 
                return false; 

            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            const string validateSql = @"SELECT dbo.EmployeeLoginValidation(@employee_id, @password)";

            await using var validateCmd = new SqlCommand(validateSql, conn);
            validateCmd.CommandType = CommandType.Text;
            validateCmd.Parameters.AddWithValue("@employee_id", employeeId); 
            validateCmd.Parameters.AddWithValue("@password", password);

            var result = await validateCmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
                return false;

            return Convert.ToInt32(result) == 1;
        }
    }
}