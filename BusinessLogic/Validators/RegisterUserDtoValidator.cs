using BusinessLogic.Dtos;
using FluentValidation;

namespace BusinessLogic.Validators;
public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator() 
    {
        RuleFor(u => u.Name)
            .NotEmpty();

        RuleFor(u => u.Email)
            .EmailAddress();

        RuleFor(u => u.Password)
            .NotEmpty();
    }
}
