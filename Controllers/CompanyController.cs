using AutoMapper;
using Castle.Core.Smtp;
using Employee_API_JWT_1035.Identity;
using FluentValidation;
using Login_Register.DTO_s;
//using Login_Register.Migrations;
using Login_Register.Models;
using Login_Register.Models.ViewModel;
using Login_Register.Repository;
using Login_Register.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;
using IEmailSender = Login_Register.Repository.IRepository.IEmailSender;

namespace Login_Register.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = SD.role_Admin + "," + SD.role_Company)]
   // [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _company;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IEmployee _employee;
        private readonly IDesignationRepository _designation;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IApplyjob _applyjob;
        public CompanyController(IEmailSender emailSender, ICompanyRepository company,
            IMapper mapper, UserManager<ApplicationUser> userManager,
            IUserRepository userRepository, IEmployee employee, ApplicationDbContext context,
            IDesignationRepository designation, IApplyjob applyjob)
        {
            _company = company;
            _mapper = mapper;
            _userManager = userManager;
            _userRepository = userRepository;
            _employee = employee;
            _emailSender = emailSender;
            _context = context;
            _designation = designation;
            _applyjob = applyjob;
        }
        [HttpGet]
        [Route("allemployee")]
        public async Task<IActionResult> getAllCompany()
        {
            var Company = await _company.Getcompanies();
            var Companydto = Company.Select(_mapper.Map<Company, Companydto>).ToList();
            return Ok(Companydto);
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
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
                var companies = await _company.Getcompanies();
                if (companies == null)
                {
                    return BadRequest(new { error = "No Companies Found." });
                }

                return Ok(companies);
            }

            var specificCompany = new List<Company>();
            var userCompany = _context.companies.FirstOrDefault(u => u.ApplicationuserId == user.Id);

            if (userCompany == null)
            {
                return BadRequest(new { error = "No Company Found." });
            }

            specificCompany.Add(userCompany);
            return Ok(specificCompany);
        }


        [HttpGet("{companyid:int}")]


        public async Task<IActionResult> GetCompany(int companyid)
        {
            var company = await _company.GetCompany(companyid);
            if (company == null) return NotFound();
            var companydto = _mapper.Map<Company>(company);
            return Ok(companydto);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Companydto companydto)
        {
            if (companydto == null)
                return BadRequest(ModelState);

            // FluentValidation
            var validator = new CompanyValidator();
            var validationResult = validator.Validate(companydto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            // Process image file
            try
            {
                if (companydto.ImageData != null && companydto.ImageData.Length > 0)
                {
                    // Set the ImageUrl to null since it will be updated in the repository
                    companydto.Image = null;
                }
                var company = _mapper.Map<Company>(companydto);
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = companydto.Name,
                    PasswordHash = "Admin@123",
                    Role = SD.role_Company
                };

                var register = await _userRepository.RegisterUser(user);
                company.ApplicationuserId = user.Id;

                var createCompany = await _company.CreateCompany(company);
                var companydtoResult = _mapper.Map<Companydto>(createCompany);


                return Ok(new { Status = "Success", data = companydtoResult, Message = "Company created successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "plz try later");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] Companydto companydto)
        {
            if (companydto == null) return BadRequest(ModelState);
            ////fluentvalidation
            var validator = new CompanyValidator();
            var validationresult = validator.Validate(companydto);
            if (!validationresult.IsValid)
            {
                return BadRequest(validationresult.Errors);
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);
            var company = _mapper.Map<Company>(companydto);
            ApplicationUser user = new ApplicationUser()
            {
                UserName = companydto.Name,
            };
            if (!await _company.UpdateCompany(company))
            {
                ModelState.AddModelError("", $"Something went wrong Update Company:{company.Name} ");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new { Status = "Success", data = companydto, Message = "Company Update successfully!" });
        }

        [HttpGet("details/{companyId}")]
        public async Task<IActionResult> GetCompanyDetailsById(string companyId)
        {
            try
            {
                var companyDetails = await _company.GetCompanyDetailsByIdAsync(companyId);

                if (companyDetails == null)
                {
                    return NotFound(); // or handle as needed
                }

                return Ok(companyDetails);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpDelete("{companyid:int}")]
        public async Task<IActionResult> DeleteCompany(int companyid)
        {
            var company = await _company.GetCompany(companyid);

            if (company == null)
            {
                return NotFound();
            }

            var employees = await _employee.GetEmployeesByCompanyId(companyid);

            foreach (var employee in employees)
            {
                var designations = await _designation.GetDesignationByEmployeeId(employee.Id);

                // Delete employee's designations
                foreach (var designation in designations)
                {
                    await _designation.DeleteDesignation(designation);
                }

                // Delete employee
                await _employee.DeleteEmployee(employee);
            }

            if (!await _company.DeleteCompany(company))
            {
                ModelState.AddModelError("", $"Something went wrong while deleting company: {company.Name}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(new { Status = "Success", Message = "Company, employees, and related designations deleted successfully!" });
        }

        //[HttpPost]
        //[Route("apply")]
        //public async Task<IActionResult> Apply([FromBody] EmailVM emailVM)
        //{
        //    if (emailVM == null || emailVM.CompanyId == null || emailVM.employeeId == null)
        //    {
        //        return BadRequest("Invalid data");
        //    }

        //    var company = await _company.GetCompany(emailVM.CompanyId.Value);
        //    var employee = await _employee.GetEmployee(emailVM.employeeId.Value);

        //    if (company == null || employee == null)
        //    {
        //        return NotFound("Company or employee not found");
        //    }

        //    var subject = "Application Submission";

        //    // Load the HTML template from the file
        //    var templatePath = Directory.GetCurrentDirectory() + "\\Template\\Email.html";
        //    var htmlTemplate = string.Empty;

        //    try
        //    {
        //        using (StreamReader stream = new StreamReader(templatePath))
        //        {
        //            htmlTemplate = stream.ReadToEnd();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle the exception, log, or throw it as needed
        //        return StatusCode(500, "Internal server error");
        //    }

        //    // Replace placeholders in the HTML template with actual values
        //    htmlTemplate = htmlTemplate.Replace("[receiverUserName]", company.Name)
        //                               .Replace("[senderUserName]", employee.Name)
        //                               .Replace("[senderId]", employee.ApplicationuserId)
        //                               .Replace("[date]", DateTime.UtcNow.ToString())
        //                               .Replace("[time]", DateTime.Now.ToShortTimeString())
        //                               .Replace("[reciverId]", company.ApplicationuserId);

        //    // Send the email
        //    await _emailSender.SendEmailAsync(company.Email, subject);

        //    return Ok("Application submitted successfully");
        //}


        //[HttpPost("apply")]
        //public async Task<IActionResult> Apply([FromBody] EmailVM emailVM)
        //{
        //    if (emailVM == null || emailVM.CompanyId == null || emailVM.employeeId == null)
        //    {
        //        return BadRequest("Invalid data");
        //    }

        //    var company = await _company.GetCompany(emailVM.CompanyId.Value);
        //    var employee = await _employee.GetEmployee(emailVM.employeeId.Value);

        //    if (company == null || employee == null)
        //    {
        //        return NotFound("Company or employee not found");
        //    }

        //    var subject = "Application Submission";

        //    // Load the HTML template from the file
        //    var templatePath = Directory.GetCurrentDirectory() + "\\Template\\Email.html";
        //    var htmlTemplate = string.Empty;

        //    try
        //    {
        //        using (StreamReader stream = new StreamReader(templatePath))
        //        {
        //            htmlTemplate = stream.ReadToEnd();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle the exception, log, or throw it as needed
        //        return StatusCode(500, "Internal server error");
        //    }

        //    // Replace placeholders in the HTML template with actual values
        //    htmlTemplate = htmlTemplate.Replace("[receiverUserName]", employee.Name)
        //                               .Replace("[senderUserName]", company.Name)
        //                               .Replace("[senderId]", employee.ApplicationuserId)
        //                               .Replace("[date]", DateTime.UtcNow.ToString())
        //                               .Replace("[time]", DateTime.Now.ToShortTimeString())
        //                               .Replace("[reciverId]", company.ApplicationuserId);

        //    var emailDto = new Emaildto
        //    {
        //        To = company.Email,
        //        Subject = subject,
        //        Body = htmlTemplate,
        //        ReceiveruserName = company.Name,
        //        SenderuserName = employee.Name,
        //        ReceiverId = company.ApplicationuserId,
        //        senderId = employee.ApplicationuserId,
        //        Number = employee.Phone,
        //        Email = employee.Email,
        //    };

        //    // Use the properties of the emailDto class as needed
        //    var toEmail = emailDto.To;
        //    var emailBody = emailDto.Body;

        //    // Call the SendEmailAsync method with emailDto
        //    _emailSender.SendEmailAsync(emailDto);

        //    // Include the company ID in the response
        //    var response = new
        //    {
        //        CompanyId = company.Id, // Assuming company has an "Id" property
        //        Message = "Application submitted successfully"
        //    };

        //    return Ok(response);
        //}



        //[HttpPost]
        //[Route("email")]
        //public IActionResult Email(Emaildto emaildto)
        //{
        //    _emailSender.SendEmailAsync(emaildto);
        //    return Ok(emaildto);
        //}


        //[HttpPost("apply")]
        //public async Task<IActionResult> Apply([FromBody] Emaildto emailVM)
        //{
        //    if (emailVM == null || emailVM.ReceiverId == null || emailVM.senderId == null)
        //    {
        //        return BadRequest("Invalid data");
        //    }

        //    var company = await _company.GetCompany(emailVM.ReceiverId.Value);
        //    var employee = await _employee.GetEmployee(emailVM.senderId.Value);

        //    if (company == null || employee == null)
        //    {
        //        return NotFound("Company or employee not found");
        //    }

        //    var applicationStatus = ApplicationStatus.Pending;

        //    // Determine the application status (approved or rejected) based on your business logic
        //    // For illustration purposes, let's assume it's approved in this example
        //    applicationStatus = ApplicationStatus.Approved;

        //    var subject = "Application Submission";
        //    var message = $"An employee has applied to your company.\n\nEmployee Name: {employee.Name}\nEmployee Email: {employee.Email}\n\nCompany Name: {company.Name}\nCompany Email: {company.Email}\n\nApplication Status: {applicationStatus}";

        //    // Send email to the company
        //    await _emailSender.SendEmailAsync(company.Email, subject, message);

        //    // Send email to the employee
        //  //  var employeeSubject = applicationStatus == ApplicationStatus.Approved ? "Application Approved" : "Application Rejected";
        //  //  var employeeMessage = applicationStatus == ApplicationStatus.Approved
        //  //      ? "Your application has been approved. Congratulations!"
        //  //      : "We regret to inform you that your application has been rejected.";

        //  //  await _emailSender.SendEmailAsync(employee.Email, employeeSubject, employeeMessage);

        //    return Ok(new { message = "Application submitted successfully", status = applicationStatus });
        //}

        //[HttpPost("send")]
        //public IActionResult SendEmail([FromBody] Emaildto emailDto)
        //{
        //    try
        //    {
        //        if (emailDto == null)
        //        {
        //            return BadRequest("Invalid email data");
        //        }

        //        // Assuming senderId corresponds to an employee and ReceiverId corresponds to a company
        //        // Customize email content based on the sender and receiver

        //        string emailContent;

        //        if (!string.IsNullOrEmpty(emailDto.senderId) && !string.IsNullOrEmpty(emailDto.ReceiverId))
        //        {

        //            emailContent = $"Dear {emailDto.ReceiveruserName}, you have a new application from employee {emailDto.SenderuserName}.";
        //        }
        //        else
        //        {

        //            return BadRequest("Invalid senderId or ReceiverId");
        //        }
        //        _emailSender.SendEmailAsync(new Emaildto
        //        {
        //            To = emailDto.To,
        //            Subject = emailDto.Subject,
        //            Body = emailDto.Body + emailContent,
        //            ReceiveruserName = emailDto.ReceiveruserName,
        //            SenderuserName = emailDto.SenderuserName,
        //            ReceiverId = emailDto.ReceiverId,
        //            senderId = emailDto.senderId,
        //            Number = emailDto.Number,
        //            Email = emailDto.Email
        //        });

        //        return Ok("Email sent successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal Server Error: {ex.Message}");
        //    }
        //}




    }
}
