
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace University_HR_ManagementSystem.Models
{
    public class EmployeeProfileViewVM
    {
        public int EmployeeID { get; set; }
        
        public string? First_name {  get; set; }
    
        public string? Last_name { get; set; }

        public char? Gender { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public int? Years_of_experience {  get; set; }

        public string? Official_day_off {  get; set; }

        public string? Type_of_contract {  get; set; }

        public string? Employment_status { get; set; }

        public int? Annual_balance { get; set; }

        public int? Accidental_balance { get; set; }



    }
}
