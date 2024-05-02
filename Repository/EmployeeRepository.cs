using Employee_API_JWT_1035.Identity;
using Login_Register.Models;
using Login_Register.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Login_Register.Repository
{
    public class EmployeeRepository : IEmployee
    {
        private readonly ApplicationDbContext _context;
        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Employee> CreateEmployee(Employee Employee)
        {
            var data = await _context.employees.AddAsync(Employee);
            await _context.SaveChangesAsync();
            return data.Entity;
        }

        public async Task<bool> DeleteEmployee(Employee Employee)
        {
            _context.employees.Remove(Employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ICollection<Employee>> GetEmployees()
        {
           
                return await _context.employees.ToListAsync();

        }

        public async Task<Employee> GetEmployee(int EmployeeId)
        {
            return await _context.employees.FindAsync(EmployeeId);
        }

        public async Task<bool> GetEmployeeExists(int EmployeeId)
        {
            return await _context.employees.AnyAsync(e => e.Id == EmployeeId);
        }

        public async Task<bool> GetEmployeeExists(string EmployeeName)
        {
            return await _context.employees.AnyAsync(e => e.Name == EmployeeName);
        }

        public async Task<bool> save()
        {
            return await _context.SaveChangesAsync() == 1 ? true : false;
        }

        public Task<bool> UpdateEmployee(Employee Employee)
        {
            _context.employees.Update(Employee);
            return save();
        }

      

       async Task<List<Employee>> IEmployee.GetEmployeesByCompanyId(int companyId)
        {
            return await  _context.employees
          .Where(e => e.CompanyId == companyId)
          .ToListAsync();
        }
    }
}
