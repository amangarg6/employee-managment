using AutoMapper;
using Employee_API_JWT_1035.Identity;
using Login_Register.DTO_s;
//using Login_Register.Migrations;
using Login_Register.Models;
using Login_Register.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Login_Register.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;
  
        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
          
   
        }

        public async Task<Company> CreateCompany(Company Company)
        {
            if (Company.ImageData != null && Company.ImageData.Length > 0)
            {
                // Generate a unique filename for the image
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Company.ImageData.FileName;
                var imagePath = Path.Combine("Images", "Images", uniqueFileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await Company.ImageData.CopyToAsync(stream);
                }

                // Update the employee's ImageUrl with the unique filename
                Company.Image = $"/Images/{uniqueFileName}";
            }
            var data = await _context.companies.AddAsync(Company);
            await _context.SaveChangesAsync();
            return data.Entity;
        }
        public async Task<bool> DeleteCompany(Company company)
        {
            var existingCompany = await _context.companies.FindAsync(company.Id);
            if (existingCompany == null)
            {
                return false;
            }
            // Delete the associated image file
            if (!string.IsNullOrEmpty(existingCompany.Image))
            {
                var imagePath = Path.Combine("Images", existingCompany.Image.TrimStart('/'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _context.companies.Remove(company);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<ICollection<Company>> Getcompanies()
        {
            return await _context.companies.ToListAsync();

        }
        public async Task<Company> GetCompany(int CompanyId)
        {
            return await _context.companies.FindAsync(CompanyId);
        }

        public async Task<CompanyDetailsdto> GetCompanyDetailsByIdAsync(string companyId)
        {
            if (!int.TryParse(companyId, out int companyIdInt))
            {             
                return null;
            }

            var companyDetails = await _context.companies
                .FirstOrDefaultAsync(c => c.Id == companyIdInt);

            if (companyDetails == null)
            {
                
                return null;
            }
            var companyDetailsDto = new CompanyDetailsdto
            {
                Id = companyDetails.Id.ToString(), 
                Name = companyDetails.Name,  
                Location= companyDetails.Location,
                Email= companyDetails.Email,
                City= companyDetails.City,  
                Description= companyDetails.Description,
                Phonenumber= companyDetails.Phonenumber,
            };

            return companyDetailsDto;
        }


        public async Task<bool> GetCompanyExists(int CompanyId)
        {
            return await _context.companies.AnyAsync(e => e.Id == CompanyId);
        }

        public async Task<bool> GetCompanyExists(string CompanyName)
        {
            return await _context.companies.AnyAsync(e => e.Name == CompanyName);
        }

        public async Task<bool> save()
        {
            return await _context.SaveChangesAsync() == 1 ? true : false;
        }

        public async Task<bool> UpdateCompany(Company updatedCompany)
        {
            var existingCompany = await _context.companies.FindAsync(updatedCompany.Id);
            if (existingCompany == null)
            {
                return false;
            }
            // Check if there is a new image to update
            if (updatedCompany.ImageData != null && updatedCompany.ImageData.Length > 0)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + updatedCompany.ImageData.FileName;
                var imagePath = Path.Combine("Images", "Images", uniqueFileName);
                if (!string.IsNullOrEmpty(existingCompany.Image))
                {
                    var previousImagePath = Path.Combine("Images", existingCompany.Image.TrimStart('/'));

                    // Delete the associated image if it exists
                    if (System.IO.File.Exists(previousImagePath))
                    {
                        System.IO.File.Delete(previousImagePath);
                    }
                }
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await updatedCompany.ImageData.CopyToAsync(stream);
                }
                existingCompany.Image = $"/Images/{uniqueFileName}";
            }
            existingCompany.Name = updatedCompany.Name;
            existingCompany.Description = updatedCompany.Description;
            existingCompany.Location = updatedCompany.Location;
            existingCompany.City = updatedCompany.City;
            existingCompany.Phonenumber = updatedCompany.Phonenumber;
            existingCompany.Email = updatedCompany.Email;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
