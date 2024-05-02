using Employee_API_JWT_1035.Identity;
using System.Security.Claims;

namespace Login_Register.Repository.IRepository
{
    public interface ITokenRepo
    {
        ApplicationUser GenerateToken(ApplicationUser user, bool isGenerateRefreshToken); 
        ClaimsPrincipal? GetClaimsFromExpiredToken(string token);
    }
}
