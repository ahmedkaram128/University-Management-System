using System.Collections.Generic;
using System.Threading.Tasks;
using University_HR_ManagementSystem.Models;

namespace University_HR_ManagementSystem.Services
{
    public interface IAdminRepository
    {
        Task<List<EmployeesPerDeptVM>> GetEmployeesPerDepartmentAsync();
        Task<List<ResignedDeductionVM>> GetResignedDeductionsAsync();
        Task<int> RemoveResignedDeductionsAsync();

        Task<string> AddHolidayAsync(string holidayName, DateTime fromDate, DateTime toDate);

        Task<List<AttendanceVM>> GetYesterdayAttendanceAsync();

        Task<List<HolidayAttendanceVM>> GetHolidayAttendanceAsync();
        Task<int> RemoveHolidayAttendanceAsync();

        Task<List<RemoveApprovedLeaveVM>> GetApprovedLeaveAttendanceAsync(int employeeId);
        Task<int> CountApprovedLeaveAttendanceAsync(int employeeId);
        Task RemoveApprovedLeavesAsync(int employeeId);

        Task<UpdateEmploymentStatusVM> UpdateEmploymentStatusAsync(int employeeId);

        Task<List<EmployeeProfileViewVM>> getAllEmployeeProfilesAsync();

        Task<List<allPerformanceVM>> getAllPerformanceAsync();

        Task<List<allRejectedMedicalsVM>> getRejectedMedicalAsync();

        Task<bool> UpdateAttendanceAsync(int employeeId, TimeSpan? checkIn, TimeSpan? checkOut);

        Task<bool> AddReplacementAsync(int emp1Id, int emp2Id, DateTime fromDate, DateTime toDate);

        Task<List<EmpReplacesEmpVM>> GetAllReplacementsAsync();

        Task<int> InitiateAttendanceAsync();

        Task<List<AttendanceVM>> GetAttendanceByDateAsync(DateTime date);

        Task<List<AttendanceVM>> RemoveDayOffAsync(int employeeId);




    }
}
