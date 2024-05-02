using Login_Register.DTO_s;
using Login_Register.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Login_Register.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplyController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IApplyjob _applyjob;
        public ApplyController(IHttpContextAccessor contextAccessor, IApplyjob applyjob)
        {
            _contextAccessor = contextAccessor;
            _applyjob = applyjob;
        }

        [HttpPost]
            public async Task<IActionResult> CreateInvitation([FromBody]FindUser invite)
            {
                if (string.IsNullOrEmpty(invite.companyId))
                    return BadRequest();

                // here we will get the token form httpcontext ........
                var token = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var getSenderId = _applyjob.GetIdFromToken(token);

                if (getSenderId == null)
                    return BadRequest(new { message = "your token doesnot contain user id " });

                var result = _applyjob.ApplyJobs(getSenderId, invite.companyName);
                return Ok(result);
            }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var token = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer", "");
            var getSenderId = _applyjob.GetIdFromToken(token);
            var data = _applyjob.GetAllRegisteredPersons(getSenderId);
            return Ok(data);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> Invitation(string username)
        {
            var token = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var getSenderId = _applyjob.GetIdFromToken(token);
            var data = await _applyjob.GetSpecificInvitations(username, getSenderId);
            return Ok(data);
        }

        [HttpGet("{reciverId}/{status}")]
        public async Task<IActionResult> Status(string reciverId, int status)

        {
            var token = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var getSenderId = _applyjob.GetIdFromToken(token);
            if (getSenderId == null || reciverId == null || status == 0) return BadRequest();

            if (!_applyjob.UpdateStatus(reciverId, getSenderId, status))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new { Status = 1, Message = "Invitation Updated Successfully" });
        }
    }
}
