using System.ComponentModel.DataAnnotations;

namespace Login_Register.Models
{
    public class TokenClass
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
