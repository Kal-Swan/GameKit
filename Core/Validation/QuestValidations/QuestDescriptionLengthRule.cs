using Core.Validation.Models;
using Domain.Entities;

namespace Core.Validation.QuestValidations;

public class QuestDescriptionLengthRule : IValidationRule<QuestEntity>
{
    public ValidationResult Validate(QuestEntity entity)
    {
        return entity.Description.Length > 1000 ? 
            ValidationResult.Failure([new ValidationError(nameof(entity.Description),"Description must be under 1000 characters long")]) : 
            ValidationResult.Success();
    }
}