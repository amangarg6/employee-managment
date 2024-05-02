using Login_Register.DTO_s;
using Login_Register.Models;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Login_Register.Repository.IRepository
{
    public interface IApplyjob
    {
        Task<ICollection<FindUser>> GetSpecificInvitations(string username, string senderId);
        ICollection<Applyjob> GetAllRegisteredPersons(string userId);
       public bool ApplyJobs(string senderId, string reciverId);
        public bool UpdateAction(string reciverId, string senderId, int action);
        public string? GetIdFromToken(string token);
        public ICollection<Applyjob> ApplyComesFromUser(string userId);
        public bool UpdateStatus(string reciverId, string senderId, int status);
        //Task<OperationResult> ApplyToCompany(int companyId, Emaildto applyRequest);
    }
}
