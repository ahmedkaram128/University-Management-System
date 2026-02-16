namespace University_HR_ManagementSystem.Models
{
    public class AddHolidayVM
    {
        public string? HolidayName { get; set; }

        public DateTime FromDate { get; set; } = DateTime.Today;

        public DateTime ToDate { get; set; } = DateTime.Today;

        public string? ErrorMessage { get; set; }
    }
}