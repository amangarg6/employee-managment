using AutoMapper;
using Employee_API_JWT_1035.Identity;
using FluentValidation;
using Login_Register.DTO_s;
using Login_Register.Models;
using Login_Register.Repository;
using Login_Register.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Security.Claims;

namespace Login_Register.Controllers
{
    [Route("api/employee")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployee _employee;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _company;
        private readonly ApplicationDbContext _context;
        public EmployeeController(IEmployee employee, IMapper mapper, IUserRepository userRepository, ICompanyRepository company, ApplicationDbContext context)
        {
            _employee = employee;
            _mapper = mapper;
            _userRepository = userRepository;
            _company = company;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //var employees = await _employee.GetEmployees();
            //var employeedto = employees.Select(_mapper.Map<Employee, Employeedto>).ToList();
            //return Ok(employeedto);
            ClaimsIdentity? claimIdentity = User?.Identity as ClaimsIdentity;
            if (claimIdentity == null)
            {
                return BadRequest(new { error = "Invalid claims identity." });
            }

            var nameClaim = claimIdentity.FindFirst(ClaimTypes.Name);
            if (nameClaim == null)
            {
                return BadRequest(new { error = "Name claim not found." });
            }

            var user = await _userRepository.CheckUserInDb(nameClaim.Value);
            if (user == null)
            {
                return BadRequest(new { error = "User not found." });
            }

            if (user.Role == "Admin")
            {
                var employees = await _employee.GetEmployees();
                if (employees == null)
                {
                    return BadRequest(new { error = "No Employees Found." });
                }

                return Ok(employees);
            }

            var specificemployee = new List<Employee>();
            var useremployee = _context.employees.FirstOrDefault(u => u.ApplicationuserId == user.Id);

            if (useremployee == null)
            {
                return BadRequest(new { error = "No Employee Found." });
            }

            specificemployee.Add(useremployee);
            return Ok(specificemployee);

        }
        [HttpGet("{employeeid:int}")]
        public async Task<IActionResult> GetEmployee(int employeeid)
        {
            var employee = await _employee.GetEmployee(employeeid);
            if (employee == null) return NotFound();
            var employeedto = _mapper.Map<Employee>(employee);
            return Ok(employeedto);
        }



        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromForm] Employeedto employeedto)
              {

            if (employeedto == null)
            {
                return BadRequest(ModelState);
            }

            // FluentValidation
            var validator = new EmployeeValidator();
            var validationResult = validator.Validate(employeedto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var employee = _mapper.Map<Employee>(employeedto);
            //employee.CompanyId = companyId;
            //if (company.Id != employeedto.CompanyId)
            //{
            //    return BadRequest("Invalid CompanyId for the employee");
            //}
            ApplicationUser user = new ApplicationUser()
            {
                UserName = employeedto.Name,
                PasswordHash = "Admin@123",
                Role = SD.role_Employee
            };

            var register = await _userRepository.RegisterUser(user);
            employee.ApplicationuserId = user.Id;

            var createEmployee = await _employee.CreateEmployee(employee);
            var employeedtto = _mapper.Map<Employeedto>(createEmployee);

            return Ok(new { Status = "Success", data = employeedtto, Message = "Employee created successfully!" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployee([FromBody] Employeedto employeedto)
        {
            if (employeedto == null) return BadRequest(ModelState);

            //FluentValidation
            var validator = new EmployeeValidator();
            var validationResult = validator.Validate(employeedto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            //**

            if (!ModelState.IsValid) return BadRequest(ModelState);
            var employee = _mapper.Map<Employee>(employeedto);
           
            ApplicationUser user = new ApplicationUser()
            {
                UserName = employeedto.Name,
            };
            if (!await _employee.UpdateEmployee(employee))
            {
                ModelState.AddModelError("", $"Something went wrong Update Employee:{employee.Name} ");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new { Status = "Success", data = employeedto, Message = "Employee Update successfully!" });

        }
        [HttpDelete("{employeeid:int}")]
        public async Task<IActionResult> DeleteEmployee(int employeeid)
        {
            if (!await _employee.GetEmployeeExists(employeeid)) return NotFound();
            var employee = await _employee.GetEmployee(employeeid);
            if (employee == null) return NotFound();
            if (!await _employee.DeleteEmployee(employee))
            {
                ModelState.AddModelError("", $"something went wrong while Delete Employee:{employee.Name}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new { Status = "Success", Message = "Employee Delete successfully!" });
        }
        [HttpGet]
        [Route ("employees")]
        public async Task<IActionResult> GetEmployeesByCompanyId(int id)
        {
            var employees = await _employee.GetEmployeesByCompanyId(id);

            if (employees == null || !employees.Any())
            {
                return NotFound("No employees found for the specified company ID");
            }

            return Ok(employees);
        }


    }
}
