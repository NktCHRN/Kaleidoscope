using BusinessLogic.Dtos;
using FluentValidation;

namespace BusinessLogic.Validators;
public class UpdateBlogDtoValidator : AbstractValidator<UpdateBlogDto>
{
    public UpdateBlogDtoValidator()
    {
        RuleFor(b => b.Name)
            .NotEmpty();

        RuleFor(b => b.Tag)
            .Must(t => t.All(c => char.IsAsciiLetter(c) || char.IsNumber(c)))
            .WithMessage("Tag must have only characters and numbers");

        RuleFor(b => b.Description)
            .MaximumLength(1000);
    }
}
