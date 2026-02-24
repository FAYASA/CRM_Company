using FluentValidation;
using seashore_CRM.Models.DTOs;

namespace seashore_CRM.BLL.Validators
{
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match.");
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Contact).MaximumLength(50);
            RuleFor(x => x.Region).MaximumLength(100);
            RuleFor(x => x.Designation).MaximumLength(100);
        }
    }
}