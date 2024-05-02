using Login_Register.Models;

namespace Login_Register.Repository.IRepository
{
    public interface IEmployee
    {

        Task <ICollection<Employee>> GetEmployees();
        Task<Employee> GetEmployee(int EmployeeId);
        Task<bool> GetEmployeeExists(int EmployeeId);
        Task<bool> GetEmployeeExists(string EmployeeName);
        Task<Employee> CreateEmployee(Employee Employee);
        Task<bool> UpdateEmployee(Employee Employee);
        Task<bool> DeleteEmployee(Employee Employee);
        Task<bool> save();
        Task<List<Employee>> GetEmployeesByCompanyId(int companyId);
    }
}
