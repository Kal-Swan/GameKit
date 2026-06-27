using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameKit.Core.Features.Entities;

namespace GameKit.ViewModels;

public partial class EntityListViewModel : ObservableObject
{
    private readonly IEntityFactory _entityFactory;
    private readonly IEntityViewModelFactory  _entityViewModelFactory;
    
    public ObservableCollection<EntityViewModel> Entities { get; } = new();

    public IReadOnlyList<string> AvailableTypes => _entityFactory.AvailableTypes;
    
    [ObservableProperty] private EntityViewModel? _selectedEntity;

    [ObservableProperty] private string? _selectedTypeToAdd;

    public EntityListViewModel(IEntityFactory entityFactory, IEntityViewModelFactory entityViewModelFactory)
    {
        _entityFactory = entityFactory;
        _entityViewModelFactory = entityViewModelFactory;
        SelectedTypeToAdd = AvailableTypes.FirstOrDefault();
    }

    [RelayCommand]
    private void AddEntity()
    {
        if (string.IsNullOrWhiteSpace(SelectedTypeToAdd))
        {
            return;
        }
        
        var entity = _entityFactory.Create(SelectedTypeToAdd);

        var viewModel = _entityViewModelFactory.CreateFor(entity);

        Entities.Add(viewModel);
        SelectedEntity = viewModel;
    }

    [RelayCommand(CanExecute = nameof(CanRemoveEntity))]
    private void RemoveEntity()
    {
        if (SelectedEntity is null)
        {
            return;
        }
        Entities.Remove(SelectedEntity);
        SelectedEntity = Entities.FirstOrDefault();
    }

    private bool CanRemoveEntity() => SelectedEntity is not null;

    partial void OnSelectedEntityChanged(EntityViewModel? value)
    {
        RemoveEntityCommand.NotifyCanExecuteChanged();
    }
}