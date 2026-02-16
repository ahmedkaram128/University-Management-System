namespace University_HR_ManagementSystem.Services
{
    public interface IAcademicRepository
    {
        Task<bool> SubmitAccidentalLeave(int empId, DateTime start, DateTime end);
        // 2. Medical
        Task<bool> SubmitMedicalLeave(int empId, DateTime start, DateTime end, string type, bool insurance, string disability, string docDesc, string fileName);

        // 3. Unpaid
        Task<bool> SubmitUnpaidLeave(int empId, DateTime start, DateTime end, string docDesc, string fileName);

        // 4. Compensation
        Task<bool> SubmitCompensationLeave(int empId, DateTime compensationDate, string reason, DateTime originalDay, int replacementId);

        // 5. Approve Unpaid Leave (Dean/VP/President)
        Task<bool> ApproveUnpaidLeave(int requestId, int adminId);

        // 6. Approve Annual Leave (Dean/VP/President)
        Task<bool> ApproveAnnualLeave(int requestId, int adminId, int replacementId);

        // 7. Evaluate Employee (Dean)
        Task<bool> EvaluateEmployee(int empId, int rating, string comment, string semester);
        Task<bool> SubmitAnnualLeave(int empId, DateTime start, DateTime end, int replacementId);
       
        Task<bool> isDean(int empId);
        Task<bool> getValidity(int dean, int emp);
    }
}
