using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using University_HR_ManagementSystem.Models;

namespace University_HR_ManagementSystem.Services
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? config.GetConnectionString("University_HR_ManagementSystemContext")
                ?? throw new InvalidOperationException(
                    "No connection string 'DefaultConnection' or 'University_HR_ManagementSystemContext' was found.");
        }


        public async Task<List<string>> GetEmployeeSemestersAsync(int employeeId)
        {
            var semesters = new List<string>();

            const string sql = @"
                SELECT DISTINCT semester
                FROM Performance
                WHERE emp_ID = @EmployeeID
                ORDER BY semester
            ";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) {
                semesters.Add(reader.GetString(0));
            }

            return semesters;
        }

        public async Task<List<SemesterPerformanceVM>> 
            GetSemesterPerformanceAsync(int employeeId, string selectedSemester)
        {
            var list = new List<SemesterPerformanceVM>();

            const string sql = @"
                SELECT semester, rating, comments  
                FROM dbo.MyPerformance(@EmployeeID, @Semester)
            ";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
            cmd.Parameters.AddWithValue("@Semester", selectedSemester);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new SemesterPerformanceVM
                {
                    Semester = reader.IsDBNull(0)?"N/A":reader.GetString(0),
                    Rating = reader.IsDBNull(1)?0:reader.GetInt32(1),
                    Comments = reader.IsDBNull(2)?"No comments":reader.GetString(2),
                });
            }

            return list;
        }
    

        public async Task<List<EmployeeAttendanceVM>> GetAttendancesAsync(int employeeId){
            var list = new List<EmployeeAttendanceVM>();

            const string sql = @"
                SELECT date, check_in_time, check_out_time, total_duration, status  
                FROM dbo.MyAttendance(@EmployeeID)
            ";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new EmployeeAttendanceVM
                {
                    Date = reader.GetDateTime(0).ToShortDateString(),
                    CheckInTime = reader.IsDBNull(1) ? "" : reader.GetTimeSpan(1).ToString(@"hh\:mm"),
                    CheckOutTime = reader.IsDBNull(2) ? "" : reader.GetTimeSpan(2).ToString(@"hh\:mm"),
                    Duration = reader.IsDBNull(3) ? "" : TimeSpan.FromMinutes(reader.GetInt32(3)).ToString(@"hh\:mm"),
                    Status = reader.GetString(4),
                });
            }

            return list;
        }

        public async Task<PayrollVM> GetEmployeePayroll(int employeeId){
            var PayrollVM = new PayrollVM();

            const string sql = @"
                select payment_date,final_salary_amount,from_date,to_date,
                    comments,bonus_amount,deductions_amount
                from dbo.Last_month_payroll(@EmployeeID)
            ";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null; 

            PayrollVM.PaymentDate =  reader.IsDBNull(0) ? "N/A" : reader.GetDateTime(0).ToShortDateString();
            PayrollVM.FinalAmount = reader.IsDBNull(1) ? 0.00M: reader.GetDecimal(1);
            PayrollVM.StartDate = reader.IsDBNull(2) ? "N/A" : reader.GetDateTime(2).ToShortDateString();
            PayrollVM.EndDate = reader.IsDBNull(3) ? "N/A" : reader.GetDateTime(3).ToShortDateString();
            PayrollVM.Comments = reader.IsDBNull(4) ? "No comments" : reader.GetString(4);
            PayrollVM.BonusAmount = reader.IsDBNull(5) ? 0.00M: reader.GetDecimal(5);
            PayrollVM.DeductionAmount = reader.IsDBNull(6) ? 0.00M: reader.GetDecimal(6);

            return PayrollVM;
        }

        public async Task<List<DeductionAttendanceVM>> GetDeductionsAttendance(int employeeId, int month){
            var list = new List<DeductionAttendanceVM>();

            const string sql = @"
                select d.date,d.amount,d.type,d.status,d.attendance_ID,a.date from 
                dbo.Deductions_Attendance(@EmployeeID,@Month) d inner join Attendance a on (d.attendance_ID=a.attendance_ID)
            ";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
            cmd.Parameters.AddWithValue("@Month", month);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new DeductionAttendanceVM
                {
                    Date = reader.IsDBNull(0)?"N/A":reader.GetDateTime(0).ToShortDateString(),
                    Amount = reader.IsDBNull(1)?0:reader.GetDecimal(1),
                    Type = reader.IsDBNull(2)?"N/A":reader.GetString(2),
                    Status = reader.GetString(3),
                    DateOfAttendance = reader.IsDBNull(4)?"N/A":reader.GetDateTime(5).ToShortDateString()
                });
            }

            return list;

        }

        public async Task<List<UnpaidLeaveVM>> GetUnpaidLeaves(int employeeId)
        {
            var list = new List<UnpaidLeaveVM>();

            const string sql = @"
                select e.employee_id,e.first_name,e.last_name,l.date_of_request,l.start_date,l.end_date,l.num_days,l.request_ID,el.status
                from  Employee_Approve_Leave el 
                inner join Leave l on (el.leave_ID=l.request_ID)
                inner join Unpaid_Leave a on (a.request_ID=l.request_ID)
                inner join Employee e on (e.employee_id=a.emp_ID)
                where el.Emp1_ID=@EmployeeID
            ";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) {
                list.Add(new UnpaidLeaveVM
                {
                    Emp_ID = reader.GetInt32(0).ToString(),
                    EmployeeName = (reader.IsDBNull(1)?"":reader.GetString(1)) + " " + (reader.IsDBNull(2)?"":reader.GetString(2)),
                    DateOfRequest = reader.IsDBNull(3)?"N/A":reader.GetDateTime(3).ToShortDateString(),
                    StartDate = reader.IsDBNull(4)?"N/A":reader.GetDateTime(4).ToShortDateString(),
                    EndDate = reader.IsDBNull(5)?"N/A":reader.GetDateTime(5).ToShortDateString(),
                    NumDays = reader.IsDBNull(6)?"N/A":reader.IsDBNull(1)?"N/A":reader.GetInt32(6).ToString(),
                    RequestID = reader.GetInt32(7),
                    Status = reader.GetString(8).ToLower()
                });
            }

            return list;

        }

        public async Task<List<AnnualLeaveVM>> GetAnnualLeaves(int employeeId)
        {
            var list = new List<AnnualLeaveVM>();

            const string sql = @"
                select e.employee_id,e.first_name,e.last_name,l.date_of_request,l.start_date,l.end_date,l.num_days,r.first_name n,r.last_name nr,r.employee_id nrr,l.request_ID,el.status,e.annual_balance
                from  Employee_Approve_Leave el 
                inner join Leave l on (el.leave_ID=l.request_ID)
                inner join Annual_Leave a on (a.request_ID=l.request_ID)
                inner join Employee r on (a.replacement_emp=r.employee_id)
                inner join Employee e on (e.employee_id=a.emp_ID)
                where el.Emp1_ID=@EmployeeID
            ";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) {
                list.Add(new AnnualLeaveVM
                {
                    Emp_ID = reader.GetInt32(0).ToString(),
                    EmployeeName = (reader.IsDBNull(1)?"":reader.GetString(1)) + " " + (reader.IsDBNull(2)?"":reader.GetString(2)),
                    DateOfRequest = reader.IsDBNull(3)?"N/A":reader.GetDateTime(3).ToShortDateString(),
                    StartDate = reader.IsDBNull(4)?"N/A":reader.GetDateTime(4).ToShortDateString(),
                    EndDate = reader.IsDBNull(5)?"N/A":reader.GetDateTime(5).ToShortDateString(),
                    NumDays = reader.IsDBNull(6)?"N/A":reader.GetInt32(6).ToString(),
                    RepEmp_Name = (reader.IsDBNull(7)?"":reader.GetString(7)) + " " + (reader.IsDBNull(8)?"":reader.GetString(8)),
                    RepEmp_ID = reader.GetInt32(9),
                    RequestID = reader.GetInt32(10),
                    Status = reader.GetString(11).ToLower(),
                    Balance = reader.IsDBNull(12)?0:reader.GetInt32(12)
                });
            }

            return list;

        }

        
        
        public async Task<bool> ValidateLeave(int requestId, int employeeId, int replacementEmp, string LeaveType)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            

            if (LeaveType == "unpaid") {
                await using var cmdd = new SqlCommand("Upperboard_approve_unpaids", conn) {
                    CommandType = System.Data.CommandType.StoredProcedure };
                cmdd.Parameters.AddWithValue("@request_ID", requestId);
                cmdd.Parameters.AddWithValue("@Upperboard_ID", employeeId);
                await cmdd.ExecuteNonQueryAsync();
            }
            else {
                await using var cmd = new SqlCommand("Upperboard_approve_annual", conn) {
                    CommandType = System.Data.CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@request_ID", requestId);
                cmd.Parameters.AddWithValue("@Upperboard_ID", employeeId);
                cmd.Parameters.AddWithValue("@replacement_ID", replacementEmp);
                await cmd.ExecuteNonQueryAsync();
            }

            const string sql = @"
                select status from Employee_Approve_Leave where leave_ID=@RequestID
            ";
            await using var cmdSelect = new SqlCommand(sql, conn) {
                CommandType = System.Data.CommandType.Text 
            };

            cmdSelect.Parameters.AddWithValue("@RequestID", requestId);
            await using var reader = await cmdSelect.ExecuteReaderAsync();
            
            if (!await reader.ReadAsync()) return false; 


            string status = reader.GetString(0);
            return status.ToLower() == "approved";        
        }

        public async Task<List<RequestStatusVM>> getRequestStatus(int employeeId){
            
            var list = new List<RequestStatusVM>();

            const string sql = @"
                select * from dbo.status_leaves(@EmployeeID)";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) {
                list.Add(new RequestStatusVM
                {   
                    RequestID = reader.GetInt32(0),
                    Status = reader.GetString(2),
                    DateOfRequest = reader.IsDBNull(1)?"N/A":reader.GetDateTime(1).ToShortDateString()
                });
            }

            return list;
            
        }
    }
}
