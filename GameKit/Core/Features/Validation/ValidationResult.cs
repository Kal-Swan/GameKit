namespace GameKit.Core.Features.Validation;

public record ValidationResult(bool IsValid, IEnumerable<string> ErrorMessages)
{
    public static ValidationResult Success()
    {
        return new ValidationResult(true, []);
    }

    public static ValidationResult Failure(IEnumerable<string> errorMessages)
    {
        return new ValidationResult(false, errorMessages);
    }
}