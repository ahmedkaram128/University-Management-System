namespace University_HR_ManagementSystem.Models
{
	public class DeductionAttendanceVM {
    public string Date { get; set; } = "N/A";
    public Decimal Amount { get; set; } = 0.00M;
    public string Type { get; set; } = "N/A";
    public string Status { get; set; } = "N/A";
    public string DateOfAttendance{get; set;} = "N/A";

	}
}
