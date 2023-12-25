using BusinessLogic.Dtos;
using FluentValidation;

namespace BusinessLogic.Validators;
public class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
{
    public UpdateCommentDtoValidator()
    {
        RuleFor(c => c.Text)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
