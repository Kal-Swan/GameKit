using Core.Validation;
using Core.Validation.Models;
using Domain.Entities;
using Domain.Entities.Tree;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Tree.Visitors;

public class ValidationNodeAction(IServiceProvider services) : ITreeNodeAction
{
    public List<ValidationError> Errors { get; } = new();
    
    public void Visit(ITreeNode node)
    {
        switch (node)
        {
            case ItemEntity item:
                var itemValidator = services.GetRequiredService<IValidator<ItemEntity>>();
                Errors.AddRange(itemValidator.Validate(item).ErrorMessages);
                break;
            case QuestEntity quest:
                var questValidator = services.GetRequiredService<IValidator<QuestEntity>>();
                Errors.AddRange(questValidator.Validate(quest).ErrorMessages);
                break;
            default:
                throw new NotSupportedException($"Validation not supported for node type: {node.GetType().Name}");
        }
    }
}