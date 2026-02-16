namespace University_HR_ManagementSystem.Models
{
    public class PayrollRecord
    {
        public int PayrollID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal FinalSalaryAmount { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Comments { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal DeductionsAmount { get; set; }
        public int EmployeeID { get; set; }
    }
}
