namespace University_HR_ManagementSystem.Models
{
    public class UnpaidLeaveVM
    {        
        public string? Emp_ID { get; set; }
        public string EmployeeName { get; set; }

        public string DateOfRequest { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public string? NumDays { get; set; }

        public int RequestID { get; set; }

        public String Status { get; set; } = "pending";
    }
}
