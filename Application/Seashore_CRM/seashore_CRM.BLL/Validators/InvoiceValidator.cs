using FluentValidation;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.BLL.Validators
{
    public class InvoiceValidator : AbstractValidator<Invoice>
    {
        public InvoiceValidator()
        {
            RuleFor(x => x.InvoiceNumber).NotEmpty();
            RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.InvoiceDate).NotEmpty();
        }
    }
}