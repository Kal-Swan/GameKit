using GameKit.Domain.Entities;

namespace GameKit.Core.Features.Entities;

public class EntityFactory : IEntityFactory
{
    private readonly Dictionary<string, Func<IEntity>> _creators = new()
    {
        ["Item"] = () => new ItemEntity(),
    };
    
    public IReadOnlyList<string> AvailableTypes => _creators.Keys.ToList();
    
    public IEntity Create(string entityType)
    {
        if (!_creators.TryGetValue(entityType, out var creator))
        {
            throw new ArgumentException($"Entity type '{entityType}' is not supported.", nameof(entityType));
        }
        
        return creator();
    }
}