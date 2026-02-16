using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using University_HR_ManagementSystem.Data;
using University_HR_ManagementSystem.Models;

namespace University_HR_ManagementSystem.Services
{
    public class AdminRepository : IAdminRepository
    {
        private readonly string _connectionString;
        private readonly University_HR_ManagementSystemContext _context;

        public AdminRepository(IConfiguration config, University_HR_ManagementSystemContext context)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? config.GetConnectionString("University_HR_ManagementSystemContext")
                ?? throw new InvalidOperationException(
                    "No connection string 'DefaultConnection' or 'University_HR_ManagementSystemContext' was found.");

            _context = context;
        }

        public async Task<List<EmployeesPerDeptVM>> GetEmployeesPerDepartmentAsync()
        {
            var result = new List<EmployeesPerDeptVM>();

            const string sql = @"SELECT * FROM NoEmployeeDept";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new EmployeesPerDeptVM
                {
                    Department = reader["Department"].ToString()!,
                    NumberOfEmployees = Convert.ToInt32(reader["Number of Employees"])
                });
            }

            return result;
        }

        public async Task<List<ResignedDeductionVM>> GetResignedDeductionsAsync()
        {
            var list = new List<ResignedDeductionVM>();

            const string sql = @"
            SELECT deduction_ID, emp_ID, date, amount, type, status
            FROM Deduction
            WHERE emp_ID IN (
                SELECT employee_id 
                FROM Employee
                WHERE employment_status = 'resigned'
                AND last_working_date < CURRENT_TIMESTAMP
            );
        ";

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new ResignedDeductionVM
                {
                    DeductionId = reader.GetInt32(0),
                    EmployeeId = reader.GetInt32(1),
                    Date = reader.GetDateTime(2),
                    Amount = reader.GetDecimal(3),
                    Type = reader.GetString(4),
                    Status = reader.GetString(5)
                });
            }

            return list;
        }

        public async Task<int> RemoveResignedDeductionsAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var countCmd = new SqlCommand(@"
        SELECT COUNT(*) FROM Deduction
        WHERE emp_ID IN (
            SELECT employee_id
            FROM Employee
            WHERE employment_status = 'resigned'
              AND last_working_date < CURRENT_TIMESTAMP
        )", conn);

            int count = (int)await countCmd.ExecuteScalarAsync();

            using var cmd = new SqlCommand("Remove_Deductions", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            await cmd.ExecuteNonQueryAsync();

            return count;
        }

        public async Task<string> AddHolidayAsync(string name, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var existing = await GetExistingHolidaysAsync(name);

                if (existing.Count == 0)
                {
                    return await InsertHolidayAsync(name, fromDate, toDate);
                }

                foreach (var h in existing)
                {
                    if (h.From == fromDate)
                    {
                        using var conn = new SqlConnection(_connectionString);
                        await conn.OpenAsync();

                        string updateSql = @"
                    UPDATE Holiday
                    SET to_date = @to
                    WHERE holiday_name = @name AND from_date = @from";

                        using var cmd = new SqlCommand(updateSql, conn);
                        cmd.Parameters.AddWithValue("@to", toDate);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@from", fromDate);

                        await cmd.ExecuteNonQueryAsync();

                        return "UPDATED";
                    }
                }

                foreach (var h in existing)
                {
                    bool overlaps =
                        !(h.To < fromDate || h.From > toDate);

                    if (overlaps)
                    {
                        return $"{name} already exists from {h.From:yyyy-MM-dd} to {h.To:yyyy-MM-dd}";
                    }
                }

                return await InsertHolidayAsync(name, fromDate, toDate);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public async Task<List<AttendanceVM>> GetYesterdayAttendanceAsync()
        {
            var list = new List<AttendanceVM>();

            const string sql = "SELECT * FROM allEmployeeAttendance";

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new AttendanceVM
                {
                    AttendanceId = reader.GetInt32(reader.GetOrdinal("attendance_ID")),
                    Date = reader.GetDateTime(reader.GetOrdinal("date")),
                    CheckInTime = reader.IsDBNull(reader.GetOrdinal("check_in_time"))
                        ? null
                        : reader.GetTimeSpan(reader.GetOrdinal("check_in_time")),
                    CheckOutTime = reader.IsDBNull(reader.GetOrdinal("check_out_time"))
                        ? null
                        : reader.GetTimeSpan(reader.GetOrdinal("check_out_time")),
                    TotalDuration = reader.IsDBNull(reader.GetOrdinal("total_duration"))
                        ? null
                        : reader.GetInt32(reader.GetOrdinal("total_duration")),
                    Status = reader.GetString(reader.GetOrdinal("status")),
                    EmployeeId = reader.GetInt32(reader.GetOrdinal("emp_ID"))
                });
            }

            return list;
        }

        // ✅ RESTORED METHOD #2
        public async Task<List<HolidayAttendanceVM>> GetHolidayAttendanceAsync()
        {
            var list = new List<HolidayAttendanceVM>();

            const string sql = @"
        SELECT A.* 
        FROM Attendance A
        WHERE EXISTS (
            SELECT 1 FROM Holiday H
            WHERE A.date BETWEEN H.from_date AND H.to_date
        );
    ";

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new HolidayAttendanceVM
                {
                    AttendanceId = reader.GetInt32(reader.GetOrdinal("attendance_ID")),
                    EmployeeId = reader.GetInt32(reader.GetOrdinal("emp_ID")),
                    Date = reader.GetDateTime(reader.GetOrdinal("date")),
                    CheckInTime = reader.IsDBNull(reader.GetOrdinal("check_in_time"))
                        ? null : reader.GetTimeSpan(reader.GetOrdinal("check_in_time")),
                    CheckOutTime = reader.IsDBNull(reader.GetOrdinal("check_out_time"))
                        ? null : reader.GetTimeSpan(reader.GetOrdinal("check_out_time")),
                    TotalDuration = reader.IsDBNull(reader.GetOrdinal("total_duration"))
                        ? null : reader.GetInt32(reader.GetOrdinal("total_duration")),
                    Status = reader.GetString(reader.GetOrdinal("status"))
                });
            }

            return list;
        }

        // ✅ RESTORED METHOD #3
        public async Task<int> RemoveHolidayAttendanceAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var countCmd = new SqlCommand(@"
        SELECT COUNT(*) FROM Attendance A
        WHERE EXISTS (
            SELECT 1 FROM Holiday H
            WHERE A.date BETWEEN H.from_date AND H.to_date
        );", conn);

            int count = (int)await countCmd.ExecuteScalarAsync();

            using var cmd = new SqlCommand("Remove_Holiday", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            await cmd.ExecuteNonQueryAsync();

            return count;
        }

        private const string ApprovedLeaveCondition = @"
    A.emp_ID = @emp AND (
        EXISTS (
            SELECT 1 FROM Leave L
            INNER JOIN Annual_Leave AL ON AL.request_ID = L.request_ID
            WHERE AL.emp_ID = @emp
              AND L.final_approval_status = 'Approved'
              AND A.date BETWEEN L.start_date AND L.end_date
        )
        OR EXISTS (
            SELECT 1 FROM Leave L
            INNER JOIN Accidental_Leave AC ON AC.request_ID = L.request_ID
            WHERE AC.emp_ID = @emp
              AND L.final_approval_status = 'Approved'
              AND A.date BETWEEN L.start_date AND L.end_date
        )
        OR EXISTS (
            SELECT 1 FROM Leave L
            INNER JOIN Compensation_Leave CL ON CL.request_ID = L.request_ID
            WHERE CL.emp_ID = @emp
              AND L.final_approval_status = 'Approved'
              AND A.date BETWEEN L.start_date AND L.end_date
        )
        OR EXISTS (
            SELECT 1 FROM Leave L
            INNER JOIN Medical_Leave ML ON ML.request_ID = L.request_ID
            WHERE ML.Emp_ID = @emp
              AND L.final_approval_status = 'Approved'
              AND A.date BETWEEN L.start_date AND L.end_date
        )
        OR EXISTS (
            SELECT 1 FROM Leave L
            INNER JOIN Unpaid_Leave UL ON UL.request_ID = L.request_ID
            WHERE UL.Emp_ID = @emp
              AND L.final_approval_status = 'Approved'
              AND A.date BETWEEN L.start_date AND L.end_date
        )
    )
";

        public async Task<List<RemoveApprovedLeaveVM>> GetApprovedLeaveAttendanceAsync(int employeeId)
        {
            var list = new List<RemoveApprovedLeaveVM>();

            string sql = $@"
        SELECT A.attendance_ID, A.emp_ID, A.date, A.status
        FROM Attendance A
        WHERE {ApprovedLeaveCondition};
    ";

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@emp", employeeId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new RemoveApprovedLeaveVM
                {
                    AttendanceId = reader.GetInt32(0),
                    EmployeeId = reader.GetInt32(1),
                    Date = reader.GetDateTime(2),
                    Status = reader.GetString(3)
                });
            }

            return list;
        }

        public async Task<int> CountApprovedLeaveAttendanceAsync(int employeeId)
        {
            string sql = $@"SELECT COUNT(*) FROM Attendance A WHERE {ApprovedLeaveCondition};";

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@emp", employeeId);

            return (int)await cmd.ExecuteScalarAsync();
        }

        public async Task RemoveApprovedLeavesAsync(int employeeId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("Remove_Approved_Leaves", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@employee_id", employeeId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<UpdateEmploymentStatusVM> UpdateEmploymentStatusAsync(int employeeId)
        {
            var vm = new UpdateEmploymentStatusVM();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            // old status
            string? oldStatus = null;

            using (var cmd = new SqlCommand(
                "SELECT employment_status FROM Employee WHERE employee_id = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", employeeId);
                var result = await cmd.ExecuteScalarAsync();

                if (result == null)
                {
                    vm.Result = "NOT_FOUND";
                    return vm;
                }

                oldStatus = result.ToString();
            }

            using (var cmd = new SqlCommand("Update_Employment_Status", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Employee_ID", employeeId);

                await cmd.ExecuteNonQueryAsync();
            }

            // new status
            string? newStatus = null;

            using (var cmd = new SqlCommand(
                "SELECT employment_status FROM Employee WHERE employee_id = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", employeeId);
                newStatus = (await cmd.ExecuteScalarAsync())?.ToString();
            }

            vm.OldStatus = oldStatus;
            vm.NewStatus = newStatus;

            // result
            vm.Result = oldStatus == newStatus ? "NO_CHANGE" : "UPDATED";
            return vm;
        }

        public async Task<string?> GetEmployeeStatusAsync(int employeeId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(@"
        SELECT employment_status
        FROM Employee
        WHERE employee_id = @id", conn);

            cmd.Parameters.AddWithValue("@id", employeeId);

            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString();
        }

        public async Task<List<EmployeeProfileViewVM>> getAllEmployeeProfilesAsync()
        {
            var result = new List<EmployeeProfileViewVM>();

            const string sql = @"SELECT * FROM allEmployeeProfiles";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new EmployeeProfileViewVM
                {
                    EmployeeID = Convert.ToInt32(reader["employee_ID"]),
                    First_name = reader["first_name"] == DBNull.Value ? null : reader["first_name"].ToString()!,
                    Last_name = reader["last_name"] == DBNull.Value ? null : reader["last_name"].ToString()!,
                    Gender = reader["gender"] == DBNull.Value ? null : Convert.ToChar(reader["gender"]),
                    Email = reader["email"] == DBNull.Value ? null : reader["email"].ToString()!,
                    Address = reader["address"] == DBNull.Value ? null : reader["address"].ToString()!,
                    Years_of_experience = reader["years_of_experience"] == DBNull.Value ? null : Convert.ToInt32(reader["years_of_experience"]),
                    Official_day_off = reader["official_day_off"] == DBNull.Value ? null : reader["official_day_off"].ToString()!,
                    Type_of_contract = reader["type_of_contract"] == DBNull.Value ? null : reader["type_of_contract"].ToString()!,
                    Employment_status = reader["employment_status"] == DBNull.Value ? null : reader["employment_status"].ToString()!,
                    Annual_balance = reader["annual_balance"] == DBNull.Value ? null : Convert.ToInt32(reader["annual_balance"]),
                    Accidental_balance = reader["accidental_balance"] == DBNull.Value ? null : Convert.ToInt32(reader["accidental_balance"])
                });
            }

            return result;
        }

        public async Task<List<allPerformanceVM>> getAllPerformanceAsync()
        {
            var result = new List<allPerformanceVM>();

            const string sql = @"SELECT * FROM allPerformance";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new allPerformanceVM
                {
                    Performance_ID = Convert.ToInt32(reader["performance_id"]),
                    Rating = Convert.ToInt32(reader["rating"]),
                    Comments = reader["comments"] == DBNull.Value ? null : reader["comments"].ToString(),
                    Semester = reader["semester"] == DBNull.Value ? null : reader["semester"].ToString(),
                    Emp_ID = reader["emp_ID"] == DBNull.Value ? null : Convert.ToInt32(reader["emp_ID"])
                });
            }

            return result;
        }

        public async Task<List<allRejectedMedicalsVM>> getRejectedMedicalAsync()
        {
            var result = new List<allRejectedMedicalsVM>();

            const string sql = @"SELECT * FROM allRejectedMedicals";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new allRejectedMedicalsVM
                {
                    Request_ID = Convert.ToInt32(reader["request_ID"]),
                    Insurance_status = reader["insurance_status"] == DBNull.Value
                      ? (bool?)null : Convert.ToBoolean(reader["insurance_status"]),
                    Disability_details = reader["disability_details"] == DBNull.Value
                      ? (string?)null : reader["insurance_status"].ToString(),
                    Type = reader["type"].ToString()!,
                    Emp_ID = reader["emp_ID"] == DBNull.Value ? null : Convert.ToInt32(reader["emp_ID"])

                });
            }

            return result;
        }
        public async Task<bool> UpdateAttendanceAsync(int employeeId, TimeSpan? checkIn, TimeSpan? checkOut)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("Update_Attendance", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Employee_id", employeeId);

            if (checkIn.HasValue)
                cmd.Parameters.AddWithValue("@check_in_time", checkIn.Value);
            else
                cmd.Parameters.AddWithValue("@check_in_time", DBNull.Value);

            if (checkOut.HasValue)
                cmd.Parameters.AddWithValue("@check_out_time", checkOut.Value);
            else
                cmd.Parameters.AddWithValue("@check_out_time", DBNull.Value);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;

        }

        public async Task<int> InitiateAttendanceAsync()
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("Initiate_Attendance", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected;
        }

        public async Task<List<EmpReplacesEmpVM>> GetAllReplacementsAsync()
        {
            var result = new List<EmpReplacesEmpVM>();
            var sql = @"SELECT * FROM dbo.Employee_Replace_Employee";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new EmpReplacesEmpVM
                {
                    Emp1_ID = Convert.ToInt32(reader["Emp1_ID"]),
                    Emp2_ID = Convert.ToInt32(reader["Emp2_ID"]),
                    FromDate = Convert.ToDateTime(reader["from_date"]),
                    ToDate = Convert.ToDateTime(reader["to_date"])
                });
            }

            return result;
        }

        public async Task<List<AttendanceVM>> GetAttendanceByDateAsync(DateTime date)
        {
            var result = new List<AttendanceVM>();

            const string sql = @"
        SELECT emp_ID, [date], check_in_time, check_out_time, status
        FROM Attendance
        WHERE [date] = @date
        ORDER BY emp_ID;
    ";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@date", date);

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new AttendanceVM
                {
                    EmployeeId = Convert.ToInt32(reader["emp_ID"]),
                    Date = Convert.ToDateTime(reader["date"]),
                    CheckInTime = reader["check_in_time"] == DBNull.Value
                                    ? (TimeSpan?)null
                                    : (TimeSpan)reader["check_in_time"],
                    CheckOutTime = reader["check_out_time"] == DBNull.Value
                                    ? (TimeSpan?)null
                                    : (TimeSpan)reader["check_out_time"],
                    Status = reader["status"].ToString()!
                });
            }

            return result;
        }

        public async Task<List<AttendanceVM>> RemoveDayOffAsync(int employeeId)
        {
            var removedList = new List<AttendanceVM>();
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string selectSql = @"
            SELECT *
            FROM Attendance A
            INNER JOIN Employee E ON E.employee_id = A.emp_ID
            WHERE A.emp_ID = @id
              AND YEAR(A.[date]) = YEAR(GETDATE())
              AND MONTH(A.[date]) = MONTH(GETDATE())
              AND UPPER(DATENAME(WEEKDAY, A.[date])) = UPPER(E.official_day_off)
              AND (
                    UPPER(A.status) = 'ABSENT'
                    OR (A.check_in_time IS NULL AND A.check_out_time IS NULL)
                  )";

            await using var cmd1 = new SqlCommand(selectSql, conn);
            cmd1.Parameters.AddWithValue("@id", employeeId);

            await using var reader = await cmd1.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                removedList.Add(new AttendanceVM
                {
                    EmployeeId = Convert.ToInt32(reader["emp_ID"]),
                    Date = Convert.ToDateTime(reader["date"]),
                    CheckInTime = reader["check_in_time"] == DBNull.Value
                                    ? (TimeSpan?)null
                                    : (TimeSpan)reader["check_in_time"],
                    CheckOutTime = reader["check_out_time"] == DBNull.Value
                                    ? (TimeSpan?)null
                                    : (TimeSpan)reader["check_out_time"],
                    Status = reader["status"].ToString()!
                });
            }

            await using var cmd = new SqlCommand("Remove_DayOff", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@employee_ID", employeeId);

            await cmd.ExecuteNonQueryAsync();

            return removedList;
        }

        public async Task<bool> AddReplacementAsync(int emp1Id, int emp2Id, DateTime fromDate, DateTime toDate)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("Replace_employee", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Emp1_ID", emp1Id);
            cmd.Parameters.AddWithValue("@Emp2_ID", emp2Id);

            cmd.Parameters.AddWithValue("@from_date", fromDate);
            cmd.Parameters.AddWithValue("@to_date", toDate);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        private async Task<List<(DateTime From, DateTime To)>> GetExistingHolidaysAsync(string name)
        {
            var list = new List<(DateTime, DateTime)>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string sql = @"SELECT from_date, to_date 
                   FROM Holiday 
                   WHERE holiday_name = @name";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", name);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add((
                    reader.GetDateTime(0),
                    reader.GetDateTime(1)
                ));
            }

            return list;
        }
        private async Task<string> InsertHolidayAsync(string name, DateTime from, DateTime to)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Add_Holiday";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@holiday_name", name);
            cmd.Parameters.AddWithValue("@from_date", from);
            cmd.Parameters.AddWithValue("@to_date", to);

            await cmd.ExecuteNonQueryAsync();
            return "SUCCESS";
        }


    }
}
