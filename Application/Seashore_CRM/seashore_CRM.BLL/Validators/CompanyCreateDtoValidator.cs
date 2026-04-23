using FluentValidation;
using seashore_CRM.BLL.DTOs;

namespace seashore_CRM.BLL.Validators
{
    public class CompanyCreateDtoValidator : AbstractValidator<CompanyCreateDto>
    {
        public CompanyCreateDtoValidator()
        {
            RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        }
    }
}