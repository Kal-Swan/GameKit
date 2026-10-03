using Core.Entities;
using Core.UndoRedo;
using Core.Validation;
using Domain;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace GameKit.ViewModels;

public class EntityViewModelFactory : IEntityViewModelFactory
{
    private readonly Dictionary<string, Func<IEntity, EntityViewModel>> entityCreator;

    public EntityViewModelFactory(IServiceProvider serviceProvider, ICommandHistory  commandHistory)
    {
        entityCreator = new Dictionary<string, Func<IEntity, EntityViewModel>>
        {
            [EntityTypeConstants.Item] = entity => new ItemEntityViewModel((ItemEntity)entity, this, serviceProvider.GetRequiredService<IEntityFactory>(), serviceProvider.GetRequiredService<IValidator<ItemEntity>>(), serviceProvider.GetRequiredService<ISelectionService>(), commandHistory),
            [EntityTypeConstants.Quest] = entity => new QuestNodeViewModel((QuestEntity)entity, this, serviceProvider.GetRequiredService<IEntityFactory>(), serviceProvider.GetRequiredService<IValidator<QuestEntity>>(), serviceProvider.GetRequiredService<ISelectionService>(), commandHistory),
        };
    }
    
    public EntityViewModel CreateFor(IEntity entity)
    {
        if (!entityCreator.TryGetValue(entity.EntityType, out var creator))
        {
            throw new ArgumentException($"Entity type '{entity.EntityType}' is not supported.", nameof(entity));
        }
        
        return creator(entity);
    }
}