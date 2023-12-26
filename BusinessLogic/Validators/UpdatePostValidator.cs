using BusinessLogic.Dtos;
using FluentValidation;

namespace BusinessLogic.Validators;
public class UpdatePostValidator : AbstractValidator<UpdatePostDto>
{
    public UpdatePostValidator()
    {
        RuleFor(p => p.Header)
            .MaximumLength(250);
        RuleFor(p => p.Subheader)
            .MaximumLength(500);
    }
}
