using GameKit.Domain.Entities;

namespace GameKit.ViewModels;

public interface IEntityViewModelFactory
{
    EntityViewModel CreateFor(IEntity entity);
}