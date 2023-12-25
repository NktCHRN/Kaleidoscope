using BusinessLogic.Dtos;
using FluentValidation;

namespace BusinessLogic.Validators;
public class PaginationParametersDtoValidator : AbstractValidator<PaginationParametersDto>
{
    public PaginationParametersDtoValidator()
    {
        RuleFor(e => e.PerPage)
            .GreaterThanOrEqualTo(1);

        RuleFor(e => e.Page)
            .GreaterThanOrEqualTo(1);
    }
}
