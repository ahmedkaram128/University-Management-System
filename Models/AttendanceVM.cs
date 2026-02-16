namespace University_HR_ManagementSystem.Models
{
    public class AttendanceVM
    {
        public int AttendanceId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public int? TotalDuration { get; set; }
        public string Status { get; set; }
        public int EmployeeId { get; set; }
        
    }
}
