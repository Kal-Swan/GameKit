using Core.Validation.Models;
using Domain.Entities;

namespace Core.Validation.ItemValidations;

public class ItemNameRequiredRule : IValidationRule<ItemEntity>
{
    public ValidationResult Validate(ItemEntity entity)
    {
        return string.IsNullOrWhiteSpace(entity.Name) ?
            ValidationResult.Failure([new ValidationError(nameof(entity.Name),"Name is required")]) :
            ValidationResult.Success();
    }
}