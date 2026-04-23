using FluentValidation;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.BLL.Validators
{
    public class CommentValidator : AbstractValidator<Comment>
    {
        public CommentValidator()
        {
            RuleFor(x => x.CommentText).NotEmpty().WithMessage("Comment text is required");
        }
    }
}