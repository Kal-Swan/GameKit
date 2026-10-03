using Core.Validation.Models;

namespace Core.Validation;

public record ValidationResult(bool IsValid, IEnumerable<ValidationError> ErrorMessages)
{
    public static ValidationResult Success()
    {
        return new ValidationResult(true, []);
    }

    public static ValidationResult Failure(IEnumerable<ValidationError> errorMessages)
    {
        return new ValidationResult(false, errorMessages);
    }
}