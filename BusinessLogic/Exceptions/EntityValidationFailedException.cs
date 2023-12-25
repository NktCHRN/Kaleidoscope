using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Exceptions;
[Serializable]
public class EntityValidationFailedException : Exception
{
    public EntityValidationFailedException(string message) : base(message) { }

    public EntityValidationFailedException(string message, Exception inner) : base(message, inner) { }

    public EntityValidationFailedException(List<ValidationFailure> failures) : base(FluentValidationFailuresToString(failures)) { }

    public EntityValidationFailedException(List<ValidationFailure> failures, Exception inner) 
        : base(FluentValidationFailuresToString(failures), inner) { }

    private static string FluentValidationFailuresToString(List<ValidationFailure> failures)
    {
        return string.Join(Environment.NewLine, failures.Select(f => f.ErrorMessage));
    }

    public EntityValidationFailedException(IEnumerable<IdentityError> failures) : base(IdentityFailuresToString(failures)) { }

    public EntityValidationFailedException(IEnumerable<IdentityError> failures, Exception inner)
        : base(IdentityFailuresToString(failures), inner) { }

    private static string IdentityFailuresToString(IEnumerable<IdentityError> failures)
    {
        return string.Join(Environment.NewLine, failures.Select(f => f.Description));
    }
}
