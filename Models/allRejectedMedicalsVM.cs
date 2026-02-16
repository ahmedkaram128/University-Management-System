namespace University_HR_ManagementSystem.Models
{
    public class allRejectedMedicalsVM
    {
        public int Request_ID { get; set; }

        public bool? Insurance_status { get; set; }

        public string? Disability_details { get; set; }

        public string Type { get; set; }

        public int? Emp_ID { get; set; }
    }
}
