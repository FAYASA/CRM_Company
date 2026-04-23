using FluentValidation;
using seashore_CRM.ApplicationLayer.DTOs;

namespace seashore_CRM.BLL.Validators
{
    public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateDtoValidator()
        {
            RuleFor(x => x.ProductName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.CategoryId).GreaterThan(0);
            RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
        }
    }
}