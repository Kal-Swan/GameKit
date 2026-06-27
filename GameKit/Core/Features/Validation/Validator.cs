using GameKit.Domain.Entities;

namespace GameKit.Core.Features.Validation;

public class Validator<T>(IEnumerable<IValidationRule<T>> validationRules) : IValidator<T>
{
    public ValidationResult Validate(T entity)
    {
        var errors = validationRules.SelectMany(rule => rule.Validate(entity).ErrorMessages).ToList();
        
        if (errors.Any())
        {
            return ValidationResult.Failure(errors);
        }
        
        return ValidationResult.Success();
    }
}