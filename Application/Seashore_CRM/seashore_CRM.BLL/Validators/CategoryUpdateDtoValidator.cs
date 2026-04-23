using FluentValidation;
using seashore_CRM.BLL.DTOs;

namespace seashore_CRM.BLL.Validators
{
    public class CategoryUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
    {
        public CategoryUpdateDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.CategoryName).NotEmpty().MaximumLength(100);
        }
    }
}