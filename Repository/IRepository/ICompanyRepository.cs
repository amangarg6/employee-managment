using Login_Register.DTO_s;
using Login_Register.Models;

namespace Login_Register.Repository.IRepository
{
    public interface ICompanyRepository
    {
        Task<ICollection<Company>> Getcompanies();
        Task<Company> GetCompany(int CompanyId);
        Task<bool> GetCompanyExists(int CompanyId);
        Task<bool> GetCompanyExists(string CompanyName);
        Task<Company> CreateCompany(Company Company);
        Task<bool> UpdateCompany(Company Company);
        Task<bool> DeleteCompany(Company Company);
        Task<bool> save();
        Task<CompanyDetailsdto> GetCompanyDetailsByIdAsync(string companyId);
    }
}
