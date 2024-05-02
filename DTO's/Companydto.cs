using Employee_API_JWT_1035.Identity;
using Login_Register.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Login_Register.DTO_s
{
    public class Companydto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }

        public string Phonenumber { get; set; }
        public string City { get; set; }
     
        //public int DesignationId { get; set; }
        //public Designation? designation { get; set; }
        public string? ApplicationuserId { get; set; }
        public ApplicationUser? Applicationuser { get; set; }
        public IFormFile ImageData { get; set; }
        public string Image { get; set; }
    }
}
