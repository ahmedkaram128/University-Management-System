using System.Collections.Generic;
using System.Threading.Tasks;
using University_HR_ManagementSystem.Models;

namespace University_HR_ManagementSystem.Services
{
    public interface IEmployeeRepository
    {
        Task<List<string>> GetEmployeeSemestersAsync(int employeeId);
        Task<List<SemesterPerformanceVM>> GetSemesterPerformanceAsync(int employeeId, string selectedSemester);
        Task<List<EmployeeAttendanceVM>> GetAttendancesAsync(int employeeId);
        Task<PayrollVM> GetEmployeePayroll(int employeeId);
        Task<List<DeductionAttendanceVM>> GetDeductionsAttendance(int employeeId, int month);
        
        Task<List<UnpaidLeaveVM>> GetUnpaidLeaves(int employeeId);
        Task<List<AnnualLeaveVM>> GetAnnualLeaves(int employeeId);
        Task<bool> ValidateLeave(int request_ID, int employeeId, int replacementEmp, string LeaveType);
        Task<List<RequestStatusVM>> getRequestStatus(int employee_id);
    }
}

