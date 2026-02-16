namespace University_HR_ManagementSystem.Models
{
    public class allPerformanceVM
    {
        public int Performance_ID { get; set; }
        
        public int Rating {  get; set; }

        public string? Comments { get; set; }

        public string? Semester { get; set; }

        public int? Emp_ID { get; set; }


    }
}