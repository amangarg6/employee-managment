using Login_Register.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee_API_JWT_1035.Identity
{
    public class ApplicationUser : IdentityUser
    {

        [NotMapped]
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenValidDate { get; set; }
        [NotMapped]
        public string? Role { get; set; }




    }
}
