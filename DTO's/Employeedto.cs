using Employee_API_JWT_1035.Identity;
using Login_Register.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Login_Register.DTO_s
{
    public class Employeedto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int Salary { get; set; }
        public int? CompanyId { get; set; }
        
        public Company? company { get; set; }
        public string? ApplicationuserId { get; set; }
        public ApplicationUser? Applicationuser { get; set; }
    }
}
