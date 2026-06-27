using GameKit.Domain.Entities;

namespace GameKit.Core.Features.Validation.ItemValidations;

public class ItemNameRequiredRule : IValidationRule<ItemEntity>
{
    public ValidationResult Validate(ItemEntity entity)
    {
        return string.IsNullOrWhiteSpace(entity.Name) ?
            ValidationResult.Failure(["Name is required"]) :
            ValidationResult.Success();
    }
}