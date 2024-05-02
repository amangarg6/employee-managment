using Employee_API_JWT_1035.Identity;
using Microsoft.OpenApi.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DisplayAttribute = System.ComponentModel.DataAnnotations.DisplayAttribute;

namespace Login_Register.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        [Display(Name = "Phone Number ")]
        public string Phonenumber { get; set; }
        public string City { get; set; }


        //public int? DesignationId { get; set; }
        //[ForeignKey("DesignationId ")]
        //public Designation? designation { get; set; }

        public string? ApplicationuserId { get; set; }
        [ForeignKey("ApplicationuserId")]
        public ApplicationUser? Applicationuser { get; set; }

        public string Image { get; set; }
        [NotMapped]
        public IFormFile ImageData { get; set; }


    }
}
