using Core.Validation.Models;
using Domain.Entities;

namespace Core.Validation.ItemValidations;

public class ItemValueRangeRule : IValidationRule<ItemEntity>
{
    public ValidationResult Validate(ItemEntity entity)
    {
        if (entity.Value >= 500)
        {
            return ValidationResult.Failure([new ValidationError(nameof(entity.Value),"Value must be under 500")]);
        }
        
        if (entity.Value < 1)
        {
            return ValidationResult.Failure([new ValidationError(nameof(entity.Value),"Value must be greater than 1")]);
        }

        return ValidationResult.Success();
    }
}