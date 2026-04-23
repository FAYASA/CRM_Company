using FluentValidation;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.BLL.Validators
{
    public class SaleValidator : AbstractValidator<Sale>
    {
        public SaleValidator()
        {
            RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SaleDate).NotEmpty();
        }
    }
}