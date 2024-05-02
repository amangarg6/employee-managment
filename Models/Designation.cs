using System.ComponentModel.DataAnnotations.Schema;

namespace Login_Register.Models
{
    public class Designation
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? employee { get; set; }
    }
}
