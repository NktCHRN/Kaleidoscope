using BusinessLogic.Dtos;
using FluentValidation;

namespace BusinessLogic.Validators;
public class CreateBlogDtoValidator : AbstractValidator<CreateBlogDto>
{
    public CreateBlogDtoValidator()
    {
        RuleFor(b => b.Tag)
            .Must(t => t.All(c => char.IsAsciiLetter(c) || char.IsNumber(c)))
            .WithMessage("Tag must have only characters and numbers");

        RuleFor(b => b.Description)
            .MaximumLength(1000);
    }
}
