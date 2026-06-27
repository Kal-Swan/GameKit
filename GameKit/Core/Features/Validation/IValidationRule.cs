namespace GameKit.Core.Features.Validation;

public interface IValidationRule<in T>
{
    ValidationResult Validate(T entity);
}