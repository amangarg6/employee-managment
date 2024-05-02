using FluentValidation;
using Login_Register.DTO_s;

namespace Login_Register
{
    public class DesignationValidator:AbstractValidator<Designationdto>
    {
        public DesignationValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Enter the Name");
        }
    }
}
