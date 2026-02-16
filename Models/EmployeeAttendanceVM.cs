namespace University_HR_ManagementSystem.Models
{
	public class EmployeeAttendanceVM {
    public string Date { get; set; }
    public string CheckInTime { get; set; } = "";
    public string CheckOutTime { get; set; } = ""; 
    public string Duration { get; set; } = "";
    public string Status { get; set; }
	}
}
