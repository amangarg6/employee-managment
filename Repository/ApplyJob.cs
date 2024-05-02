using Employee_API_JWT_1035.Identity;
using Login_Register.DTO_s;
using Login_Register.Models;
using Login_Register.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Login_Register.Repository
{
    public class ApplyJob : IApplyjob
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _email;
        public ApplyJob(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender email)
        {
            _context = context;
            _userManager = userManager;
            _email = email;
        }

        public ICollection<Applyjob> ApplyComesFromUser(string userId)
        {
            return _context.Set<Applyjob>().Include(u => u.User1)
              .Where(u => u.ReciverId == userId && u.UserStatus == Applyjob.Status.Approved)
              .Select(u => new Applyjob()
              {
                  SenderId = u.SenderId,
                  User1 = new ApplicationUser()
                  {
                      Id = u.User1.Id,
                      UserName = u.User1.UserName
                  },
                  UserActions = u.UserActions,
                  UserStatus = u.UserStatus
              }
              )
              .ToList();
        }

        public bool ApplyJobs(string senderId, string reciverId)
        {
            var validatesender = _userManager.FindByIdAsync(senderId).Result;
            var validateReceiver = _userManager.FindByIdAsync(reciverId).Result;
            if (validateReceiver == null || validatesender == null)
                return false;
            // now we will create invitation here...............................
            Applyjob invitation = new Applyjob()
            {
                ReciverId = validateReceiver.Id,
                SenderId = validatesender.Id,
                UserStatus = Applyjob.Status.Pending,
                UserActions = Applyjob.Action.Enable
            };

            var receiverCheck = _context.applyjobs.FirstOrDefault(u => u.ReciverId == reciverId);
            if (receiverCheck != null) { return false; }

            _context.applyjobs.Add(invitation);
            var mailrequest = new Emaildto()
            {
                To = "aman-gupta@cssoftsolutions.com",  //validateReceiver.Email,
                Subject = "apply job",
                ReceiveruserName = validateReceiver.UserName,
                SenderuserName = validatesender.UserName,
                ReceiverId = invitation.ReciverId,
                Number = validatesender.PhoneNumber,
                Email = validatesender.Email,
            };
            _email.SendEmailAsync(mailrequest);
            return _context.SaveChanges() == 1 ? true : false;
        }

        public ICollection<Applyjob> GetAllRegisteredPersons(string userId)
        {
            var data = _context.Set<Applyjob>().Include(m => m.User2).Where(u => u.SenderId == userId).
           Select(u => new Applyjob()
           {
                 ReciverId = u.ReciverId,
                 SenderId = u.SenderId,
                 User2 = new ApplicationUser()
           {
                 UserName = u.User2.UserName
           },
                 UserStatus = u.UserStatus,
                 UserActions = u.UserActions
           }).
                 ToList();
               return data;
        }

        public string? GetIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var data = tokenHandler.ReadJwtToken(token);
            var userName = data.Claims.FirstOrDefault(x => x.Type == "unique_name")?.Value;
            return userName;
        }

        public async Task<ICollection<FindUser>> GetSpecificInvitations(string username, string senderId)
        {
            var data = await _userManager.FindByIdAsync(senderId);
            var dbusers = _userManager.Users;

            var result = dbusers.Where(u => u.UserName.Contains(username.ToUpper()) || u.UserName.Contains(username.ToLower()))
            .Select(m => new FindUser { companyId = m.Id, companyName = m.UserName }).Where(u => u.companyId != data.Id).ToList();

            return result;
        }

        public bool UpdateAction(string reciverId, string senderId, int action)
        {
            var sender = _userManager.FindByIdAsync(senderId).Result;
            var reciver = _userManager.FindByIdAsync(reciverId).Result;
            if (reciver == null || sender == null) return false;
            var findInvitation = _context.applyjobs.FirstOrDefault(m => m.SenderId == senderId && m.ReciverId == reciverId);
            if (findInvitation == null) return false;
            findInvitation.UserActions = (Applyjob.Action)action;
            if (findInvitation.UserActions == Applyjob.Action.Delete)
            {
                _context.applyjobs.Remove(findInvitation);
                return _context.SaveChanges() == 1 ? true : false;
            }
            _context.Update(findInvitation);
            return _context.SaveChanges() == 1 ? true : false;
        }

        public bool UpdateStatus(string reciverId, string senderId, int status)
        {
            var sender = _userManager.FindByIdAsync(senderId).Result;
            var reciver = _userManager.FindByIdAsync(reciverId).Result;
            if (reciver == null || sender == null) return false;
            var findInvitation = _context.applyjobs.FirstOrDefault(m => m.SenderId == senderId && m.ReciverId == reciverId);
            if (findInvitation == null) return false;
            findInvitation.UserStatus = (Applyjob.Status)status;
            if (findInvitation.UserStatus == Applyjob.Status.Approved)
                findInvitation.UserActions = Applyjob.Action.Enable;

            _context.Update(findInvitation);
            return _context.SaveChanges() == 1 ? true : false;
        }
    }
}
