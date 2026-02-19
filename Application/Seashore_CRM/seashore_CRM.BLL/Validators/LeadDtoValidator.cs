using FluentValidation;
using seashore_CRM.Models.DTOs;

namespace seashore_CRM.BLL.Validators
{
    public class LeadDtoValidator : AbstractValidator<LeadDto>
    {
        public LeadDtoValidator()
        {
            RuleFor(x => x.LeadType).NotEmpty().WithMessage("Lead Type is required.");
            RuleFor(x => x.Priority).NotEmpty().WithMessage("Priority is required.");
            RuleFor(x => x.AssignedUserId).GreaterThanOrEqualTo(0).When(x => x.AssignedUserId.HasValue);
        }
    }
}
