namespace University_HR_ManagementSystem.Models
{
    public class PayrollVM
    {
        public string PaymentDate { get; set; } = "N/A";
        public Decimal FinalAmount { get; set; }
        public string StartDate { get; set; } = "N/A";
        public string EndDate { get; set; } = "N/A";
        public string Comments { get; set; } = "No Comments Yet.";
        public Decimal BonusAmount { get; set; } 
        public Decimal DeductionAmount { get; set; } 

    }
}
