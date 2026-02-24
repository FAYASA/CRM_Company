using FluentValidation;
using seashore_CRM.Models.DTOs;

namespace seashore_CRM.BLL.Validators
{
    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Contact).MaximumLength(50);
            RuleFor(x => x.Region).MaximumLength(100);
            RuleFor(x => x.Designation).MaximumLength(100);
            RuleFor(x => x.NewPasswordConfirm).Equal(x => x.NewPassword).When(x => !string.IsNullOrEmpty(x.NewPassword)).WithMessage("New password and confirmation do not match.");
        }
    }
}