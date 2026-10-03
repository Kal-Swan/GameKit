using System.Collections.ObjectModel;

namespace GameKit.ViewModels;

public interface ISelectionService
{
    EntityViewModel? Current { get; }
    void Select(EntityViewModel? entity);
    ObservableCollection<EntityViewModel> AllRootEntities { get; }
}