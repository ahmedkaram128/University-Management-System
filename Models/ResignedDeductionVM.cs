namespace University_HR_ManagementSystem.Models
{
    public class ResignedDeductionVM
    {
        public int DeductionId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
