using FluentValidation;
using Login_Register.DTO_s;

namespace Login_Register
{
    public class CompanyValidator : AbstractValidator<Companydto>
    {
        public CompanyValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Enter the Name");
            RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Fill the email");
            RuleFor(x => x.Phonenumber).NotEmpty().WithMessage("Please specify a phone number.");
            RuleFor(x => x.Location).NotNull().NotEmpty().WithMessage("Enter the Location");
            RuleFor(x => x.City).NotNull().NotEmpty().WithMessage("Enter the City");
            RuleFor(x => x.Description).NotNull().NotEmpty().WithMessage("Enter the Description");
            //RuleFor(x => x.EmployeeId).NotNull().NotEmpty().WithMessage("Enter the Employee'Id");
            //RuleFor(x => x.DesignationId).NotNull().NotEmpty().WithMessage("Enter the Designation'Id");
        }
    }
}
