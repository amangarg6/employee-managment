using Employee_API_JWT_1035.Identity;
//using Login_Register.Migrations;
using Login_Register.Models;
using Login_Register.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Login_Register.Repository
{
    public class DesignationRepository : IDesignationRepository
    {
        private readonly ApplicationDbContext _context;
        public DesignationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Designation> CreateDesignation(Designation Designation)
        {
            var data = await _context.designations.AddAsync(Designation);
            await _context.SaveChangesAsync();
            return data.Entity;
        }

        public async Task<bool> DeleteDesignation(Designation Designation)
        {
            _context.designations.Remove(Designation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ICollection<Designation>> GetDesignations()
        {
            return await _context.designations.ToListAsync();
        }

        public async Task<Designation> GetDesignation(int DesignationId)
        {

            return await _context.designations.FindAsync(DesignationId);
        }
        public async Task<bool> save()
        {
            return await _context.SaveChangesAsync() == 1 ? true : false;
        }

        public Task<bool> UpdateDesignation(Designation Designation)
        {
            _context.designations.Update(Designation);
            return save();
        }

        async Task<List<Designation>> IDesignationRepository.GetDesignationByEmployeeId(int employeeId)
        {
            return await _context.designations
                .Where(d => d.EmployeeId == employeeId)
                .ToListAsync();
        }

    }
}
