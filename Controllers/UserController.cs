using AutoMapper;
using Azure;
using Employee_API_JWT_1035.Identity;
using Login_Register.Models;
using Login_Register.Models.ViewModel;
using Login_Register.Repository;
using Login_Register.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Response = Login_Register.Models.Response;


namespace Login_Register.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ITokenRepo _jwtManager;
        private readonly IUserRepository _userRepository;     
        private readonly IMapper _mapper;
 

        public UserController(IUserRepository userRepository, IMapper mapper, ITokenRepo jwtManager)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtManager = jwtManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginVM user)
        {
            if (await _userRepository.IsUnique(user.UserName)) return BadRequest("Please Register");
            var userAuthorize = await _userRepository.AuthenticateUser(user.UserName, user.Password);
            if (userAuthorize == null) return NotFound("Invalid Attempt");
            return Ok(userAuthorize);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] Register userRegister)
        {
            if (userRegister == null || !ModelState.IsValid) return BadRequest();
            var ApplicationUser = _mapper.Map<ApplicationUser>(userRegister);
            ApplicationUser.PasswordHash = userRegister.Password;
            ApplicationUser.Role = SD.role_Employee;
            if (!await _userRepository.IsUnique(userRegister.UserName)) return NotFound("Go to login");
            var registerUser = await _userRepository.RegisterUser(ApplicationUser);
            if (!registerUser) return BadRequest("Register First");
            return Ok("Register Successfully");
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenClass userToken)
        {
            if (userToken == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var claimToken = _jwtManager.GetClaimsFromExpiredToken(userToken.Token);
            if (claimToken == null)
            {
                return BadRequest();
            }

            var claimIdentity = claimToken.Identity as ClaimsIdentity;
            var claimUser = claimIdentity?.FindFirst(ClaimTypes.Name) ?? null;
            if (claimUser == null)
            {
                return Unauthorized();
            }

            var checkInDb = await _userRepository.CheckUserInDb(claimUser.Value);
            if (checkInDb == null) { return BadRequest(); }
            if (checkInDb.RefreshToken != userToken.RefreshToken) return Unauthorized("Go Login First");
            if (checkInDb.RefreshTokenValidDate < DateTime.Now) return BadRequest();
            var generateNewToken = _jwtManager.GenerateToken(checkInDb, false);

            TokenClass usertoken = new TokenClass()
            {
                Token = generateNewToken?.Token ?? "null",
                RefreshToken = generateNewToken?.RefreshToken ?? "null",
            };

            return Ok(usertoken);
        }

        [HttpPost("Search")]
        public async Task<IActionResult> GetCompany([FromBody] string user)
        {
            var checkInDb = await _userRepository.CheckCompanyInDb(user);
            if (checkInDb == null) { return BadRequest(); }
            return Ok(checkInDb);
        }
    }
}
