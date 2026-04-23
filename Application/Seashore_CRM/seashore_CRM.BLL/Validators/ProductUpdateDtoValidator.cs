using FluentValidation;
using seashore_CRM.ApplicationLayer.DTOs;

namespace seashore_CRM.BLL.Validators
{
    public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.ProductName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.CategoryId).GreaterThan(0);
            RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
        }
    }
}