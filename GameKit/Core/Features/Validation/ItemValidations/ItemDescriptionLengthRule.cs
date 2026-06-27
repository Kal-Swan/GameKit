using GameKit.Domain.Entities;

namespace GameKit.Core.Features.Validation.ItemValidations;

public class ItemDescriptionLengthRule : IValidationRule<ItemEntity>
{
    public ValidationResult Validate(ItemEntity entity)
    {
        return entity.Description.Length > 500 ? ValidationResult.Failure(["Description must be under 500 characters long"]) : ValidationResult.Success();
    }
}