using Core.Validation.Models;
using Domain.Entities;

namespace Core.Validation.QuestValidations;

public class QuestNameRequiredRule : IValidationRule<QuestEntity>
{
    public ValidationResult Validate(QuestEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            return ValidationResult.Failure([new ValidationError(nameof(entity.Name), "Quest name is required")]);
        }

        return ValidationResult.Success();
    }
}