using Login_Register.Models;

namespace Login_Register.Repository.IRepository
{
    public interface IDesignationRepository
    {
        Task<ICollection<Designation>> GetDesignations();
        Task<Designation> GetDesignation(int DesignationId);
        Task<Designation> CreateDesignation(Designation Designation);
        Task<bool> UpdateDesignation(Designation Designation);
        Task<bool> DeleteDesignation(Designation Designation);
        Task<List<Designation>> GetDesignationByEmployeeId(int employeeId);
        Task<bool> save();
    }
}
