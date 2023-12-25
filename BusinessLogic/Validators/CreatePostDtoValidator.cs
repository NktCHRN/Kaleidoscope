using BusinessLogic.Dtos;
using FluentValidation;

namespace BusinessLogic.Validators;
public class CreatePostDtoValidator : AbstractValidator<CreatePostDto>
{
    public CreatePostDtoValidator()
    {
        RuleFor(p => p.Header)
            .MaximumLength(250);
        RuleFor(p => p.Subheader)
            .MaximumLength(500);
    }
}
