using GameKit.Core.Features.Validation;
using GameKit.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace GameKit.ViewModels;

public class EntityViewModelFactory : IEntityViewModelFactory
{
    private readonly Dictionary<string, Func<IEntity, EntityViewModel>> entityCreator;

    public EntityViewModelFactory(IServiceProvider serviceProvider)
    {
        entityCreator = new Dictionary<string, Func<IEntity, EntityViewModel>>
        {
            ["Item"] = entity => new ItemEntityViewModel((ItemEntity)entity, serviceProvider.GetRequiredService<IValidator<ItemEntity>>()),
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