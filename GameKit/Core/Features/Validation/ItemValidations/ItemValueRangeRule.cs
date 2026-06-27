using GameKit.Domain.Entities;

namespace GameKit.Core.Features.Validation.ItemValidations;

public class ItemValueRangeRule : IValidationRule<ItemEntity>
{
    public ValidationResult Validate(ItemEntity entity)
    {
        if (entity.Value >= 500)
        {
            return ValidationResult.Failure(["Value must be under 500"]);
        }

        if (entity.Value < 1)
        {
            return ValidationResult.Failure(["Value must be greater than 1"]);
        }

        return ValidationResult.Success();
    }
}