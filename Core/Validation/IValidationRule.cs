namespace Core.Validation;

public interface IValidationRule<in T>
{
    ValidationResult Validate(T entity);
}