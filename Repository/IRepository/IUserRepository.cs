using Employee_API_JWT_1035.Identity;
using Login_Register.Models;
using Login_Register.Models.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Login_Register.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<bool> IsUnique(string userName);
        Task<ApplicationUser?> AuthenticateUser(string userName, string userPassword);
        Task<bool> RegisterUser(ApplicationUser user);
        Task<ApplicationUser?> AddOrUpdateUserRefreshToken(ApplicationUser user);
        Task<ApplicationUser?> CheckUserInDb(string userName);
        Task<ApplicationUser?> CheckCompanyInDb(string userName);



    }
}
