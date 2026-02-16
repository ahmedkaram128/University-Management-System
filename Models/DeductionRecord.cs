namespace University_HR_ManagementSystem.Models
{
    public class DeductionRecord
    {
        public int DeductionID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}