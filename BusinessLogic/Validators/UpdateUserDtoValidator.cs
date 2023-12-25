using BusinessLogic.Dtos;
using FluentValidation;

namespace BusinessLogic.Validators;
public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(u => u.Name)
            .NotEmpty();
    }
}
