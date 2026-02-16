using System.ComponentModel.DataAnnotations;

namespace University_HR_ManagementSystem.Models
{
    public class SemesterPerformanceVM
    {
        public String Semester { get; set; }

        public int Rating { get; set; }
        public String Comments { get; set; } = "No comments yet";
    }
}
