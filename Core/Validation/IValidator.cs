namespace Core.Validation;

public interface IValidator<in T>
{
    ValidationResult Validate(T entity);
}