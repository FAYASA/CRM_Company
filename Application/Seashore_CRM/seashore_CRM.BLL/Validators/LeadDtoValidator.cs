using FluentValidation;
using seashore_CRM.BLL.DTOs;

namespace seashore_CRM.BLL.Validators
{
    public class LeadDtoValidator : AbstractValidator<LeadDto>
    {
        public LeadDtoValidator()
        {
            RuleFor(x => x.LeadType)
                .NotEmpty().WithMessage("Lead Type is required.")
                .Must(t => t == "Corporate" || t == "Individual")
                .WithMessage("Lead Type must be 'Corporate' or 'Individual'.");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Priority is required.");

            // Require company for corporate leads, contact for individual leads
            When(x => !string.IsNullOrWhiteSpace(x.LeadType) && x.LeadType == "Corporate", () =>
            {
                RuleFor(x => x.CompanyId).NotNull().WithMessage("Company is required for corporate leads.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.LeadType) && x.LeadType == "Individual", () =>
            {
                RuleFor(x => x.ContactId).NotNull().WithMessage("Contact is required for individual leads.");
            });

            RuleFor(x => x.Probability)
                .InclusiveBetween(0, 100).When(x => x.Probability.HasValue)
                .WithMessage("Probability must be between 0 and 100.");

            RuleFor(x => x.Budget)
                .GreaterThanOrEqualTo(0).When(x => x.Budget.HasValue)
                .WithMessage("Budget must be >= 0.");

            RuleForEach(x => x.ProductItems).ChildRules(items =>
            {
                items.RuleFor(pi => pi.LeadProductId).GreaterThan(0).When(pi => pi.LeadProductId.HasValue).WithMessage("Invalid product selected.");
                items.RuleFor(pi => pi.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
                items.RuleFor(pi => pi.UnitPrice).GreaterThanOrEqualTo(0).WithMessage("Unit price must be >= 0.");
            });

            RuleFor(x => x.AssignedUserId).GreaterThan(0).When(x => x.AssignedUserId.HasValue).WithMessage("Assigned user id must be positive.");
        }
    }
}
