using Employee_API_JWT_1035.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Login_Register.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int Salary { get; set; }

        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? company { get; set; }
  
        public string? ApplicationuserId { get; set; }
        [ForeignKey("ApplicationuserId")]
        public ApplicationUser? Applicationuser { get; set; }
    }
}
