using Domain.Entities;

namespace Core.Entities;

public interface IEntityFactory
{
    IEntity Create(string entityType);
    IReadOnlyList<string> AvailableTypes { get; }
}