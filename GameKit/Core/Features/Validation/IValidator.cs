namespace GameKit.Core.Features.Validation;

public interface IValidator<in T>
{
    ValidationResult Validate(T entity);
}