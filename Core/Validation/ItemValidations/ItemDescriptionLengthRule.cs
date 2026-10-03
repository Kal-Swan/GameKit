using Domain.Entities;
using ValidationError = Core.Validation.Models.ValidationError;

namespace Core.Validation.ItemValidations;

public class ItemDescriptionLengthRule : IValidationRule<ItemEntity>
{
    public ValidationResult Validate(ItemEntity entity)
    {
        return entity.Description.Length > 500 ? 
            ValidationResult.Failure([new ValidationError(nameof(entity.Description),"Description must be under 500 characters long")]) : 
                ValidationResult.Success();
    }
}