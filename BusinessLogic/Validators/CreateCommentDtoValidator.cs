using BusinessLogic.Dtos;
using FluentValidation;

namespace BusinessLogic.Validators;
public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentDtoValidator()
    {
        RuleFor(c => c.Text)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
