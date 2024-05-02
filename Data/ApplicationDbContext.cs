
using Login_Register.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Employee = Login_Register.Models.Employee;

namespace Employee_API_JWT_1035.Identity
{
  public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      :base(options)
    {
        
    }
    //protected override void OnModelCreating(ModelBuilder builder)
    //{
    //  base.OnModelCreating(builder);
    //}

        public DbSet<Employee>employees { get; set; }
        public DbSet<Designation>designations { get; set; }
        public DbSet<Company> companies { get; set; }
        public DbSet<Applyjob> applyjobs { get; set; }


    }
}
