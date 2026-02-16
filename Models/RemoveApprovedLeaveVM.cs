namespace University_HR_ManagementSystem.Models
{
    public class RemoveApprovedLeaveVM
    {
        public int AttendanceId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
