using GameKit.Domain.Entities;

namespace GameKit.Core.Features.Entities;

public interface IEntityFactory
{
    IEntity Create(string entityType);
    IReadOnlyList<string> AvailableTypes { get; }
}