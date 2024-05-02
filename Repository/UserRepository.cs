using Employee_API_JWT_1035.Identity;
using Login_Register.Models;
using Login_Register.Models.ViewModel;
using Login_Register.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Login_Register.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenRepo _tokenGenrator;
        private readonly JWTSetting _jwtSetting;
        private readonly RoleManager<IdentityRole> _rolemanager;
        public UserRepository(SignInManager<ApplicationUser> signInManager,
      UserManager<ApplicationUser> userManager, ITokenRepo tokenGenrator, 
      IOptions<JWTSetting> jwtSetting, RoleManager<IdentityRole> rolemanager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenGenrator = tokenGenrator;
            _jwtSetting = jwtSetting.Value;
            _rolemanager = rolemanager;
        }

        public async Task<ApplicationUser?> AddOrUpdateUserRefreshToken(ApplicationUser user)
        {
            user.RefreshTokenValidDate = DateTime.UtcNow.AddDays(_jwtSetting.RefreshTokenExpireDays);
            var userD = await _userManager.UpdateAsync(user);
            return userD.Succeeded ? user : null;
        }

        public async Task<ApplicationUser?> AuthenticateUser(string userName, string userPassword )
        {
            var userExist = await _userManager.FindByNameAsync(userName);
            var userVerification = await _signInManager.CheckPasswordSignInAsync(userExist, userPassword, false);
            if (!userVerification.Succeeded) return null;
            if (userExist.RefreshTokenValidDate < DateTime.Now)
            {
                var userToken = _tokenGenrator.GenerateToken(userExist, true);
                //var userRole= _rolemanager.FindByNameAsync(userExist.Role);
                return await AddOrUpdateUserRefreshToken(userToken);
            }
            return _tokenGenrator.GenerateToken(userExist, false);
        }
       

        public async Task<ApplicationUser?> CheckUserInDb(string userName)
        {
            var UserInDb = await _userManager.FindByIdAsync(userName);
            if (UserInDb == null) return null;
            var rolecheck = await _userManager.GetRolesAsync(UserInDb);
            UserInDb.Role = rolecheck?.FirstOrDefault();
            return UserInDb;
        }

        public async Task<bool> IsUnique(string userName)
        {
            var userExist = await _userManager.FindByNameAsync(userName);
            if (userExist == null) return true;
            var rolecheck = await _userManager.GetRolesAsync(userExist);
            userExist.Role = rolecheck?.FirstOrDefault();
            return false;
        }
        public async Task<bool> RegisterUser(ApplicationUser user)
        {
           
            if (await _rolemanager.FindByNameAsync(user.Role)==null) return false;
            //Admin role
            //if (user.Role == SD.role_Admin)
            //{
            //    var checkAdmin = await _userManager.GetUsersInRoleAsync(SD.role_Admin);
            //    if (checkAdmin.Count == 1) return false;
            //}
            var users = await _userManager.CreateAsync(user, user.PasswordHash);
            if (!users.Succeeded) return false;
            await _userManager.AddToRoleAsync(user , user.Role);
            return true;
        }

        public async Task<ApplicationUser?> CheckCompanyInDb(string userName)
        {
            var UserInDb = await _userManager.FindByNameAsync(userName);
            if (UserInDb == null) return null;
            var rolecheck = await _userManager.GetRolesAsync(UserInDb);
            UserInDb.Role = rolecheck?.FirstOrDefault();
            return UserInDb;
        }
    }
}

