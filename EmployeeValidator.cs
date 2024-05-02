using FluentValidation;
using Login_Register.DTO_s;

namespace Login_Register
{
    public class EmployeeValidator : AbstractValidator<Employeedto>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Enter the Name");
            //RuleFor(x => x.Name).Length(4, 250);
            RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Fill the email");
            //RuleFor(x => x.Email).Length(10, 250);
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Please specify a phone number.");
            RuleFor(x => x.Address).NotNull().NotEmpty().WithMessage("Enter the Address");
        }
    }
}
