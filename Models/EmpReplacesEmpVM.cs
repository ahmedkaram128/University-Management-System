using Microsoft.VisualBasic;

namespace University_HR_ManagementSystem.Models
{
    public class EmpReplacesEmpVM
    {
        public int TableId { get; set; }
        public int Emp1_ID { get; set; }
        public int Emp2_ID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}