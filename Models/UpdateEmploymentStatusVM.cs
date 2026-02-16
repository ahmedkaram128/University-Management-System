namespace University_HR_ManagementSystem.Models
{
    public class UpdateEmploymentStatusVM
    {
        public string Result { get; set; } = "";   // NOT_FOUND / NO_CHANGE / UPDATED
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
    }
}
