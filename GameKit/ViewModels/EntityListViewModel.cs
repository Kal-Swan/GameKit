using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entities;
using Core.UndoRedo;
using GameKit.Commands;
using GameKit.Views;

namespace GameKit.ViewModels;

public partial class EntityListViewModel : ObservableObject, ISelectionService
{
    private readonly IEntityFactory _entityFactory;
    private readonly IEntityViewModelFactory  _entityViewModelFactory;
    private readonly ICommandHistory _commandHistory;

    public ObservableCollection<EntityViewModel> Entities { get; } = new();

    public IReadOnlyList<string> AvailableTypes => _entityFactory.AvailableTypes;
    
    [ObservableProperty] private EntityViewModel? _selectedEntity;
    
    EntityViewModel? ISelectionService.Current => SelectedEntity;

    [ObservableProperty] private string? _selectedTypeToAdd;

    public EntityListViewModel(IEntityFactory entityFactory,
        IEntityViewModelFactory entityViewModelFactory,
        ICommandHistory commandHistory)
    {
        _entityFactory = entityFactory;
        _entityViewModelFactory = entityViewModelFactory;
        _commandHistory = commandHistory;
        SelectedTypeToAdd = AvailableTypes.FirstOrDefault();
    }
    
    void ISelectionService.Select(EntityViewModel? entity)
    {
        SelectedEntity = entity;
    }

    public ObservableCollection<EntityViewModel> AllRootEntities => Entities;

    public void CommitPendingEdits() => SelectedEntity?.CommitPendingEdits();

    [RelayCommand]
    private void AddEntity()
    {
        if (string.IsNullOrWhiteSpace(SelectedTypeToAdd))
        {
            return;
        }
        
        var entity = _entityFactory.Create(SelectedTypeToAdd);

        var viewModel = _entityViewModelFactory.CreateFor(entity);

        var command = new AddEntityCommand(
            Entities,
            viewModel,
            vm => SelectedEntity = vm,
            SelectedEntity);
        _commandHistory.Execute(command);
    }

    [RelayCommand(CanExecute = nameof(CanRemoveEntity))]
    private void RemoveEntity()
    {
        if (SelectedEntity is null)
        {
            return;
        }
        
        TryRemoveEntity(Entities,  SelectedEntity);
    }
    
    public bool TryRemoveEntity(ObservableCollection<EntityViewModel> entities, EntityViewModel target)
    {
        if (entities.Contains(target))
        {
            var command = new DeleteEntityListCommand(
                entities,
                target,
                vm => SelectedEntity = vm,
                GetNextSelection(entities, target));

            _commandHistory.Execute(command);
            return true;
        }

        foreach (var entity in entities)
        {
            if (TryRemoveEntity(entity.Children, target))
            {
                return true;
            }
        }

        return false;
    }
    
    public bool CanMove(EntityViewModel dragged, EntityViewModel? target)
    {
        if (target is null) return dragged.Parent is not null; 
        if (ReferenceEquals(target, dragged)) return false;
        if (ReferenceEquals(target, dragged.Parent)) return false;
        return !target.IsDescendantOf(dragged);
    }

    private EntityViewModel GetNextSelection(IList<EntityViewModel> sibling, EntityViewModel viewModelToRemove)
    {
        var index = sibling.IndexOf(viewModelToRemove);

        if (index < 0)
        {
            return viewModelToRemove.Parent;
        }

        if (index + 1 < sibling.Count)
        {
            return sibling[index + 1];
        }
        
        if (index > 0)
        {
            return sibling[index - 1];
        }

        return viewModelToRemove.Parent;
    }

    private bool CanRemoveEntity() => SelectedEntity is not null;

    partial void OnSelectedEntityChanged(EntityViewModel? value)
    {
        RemoveEntityCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    public void CreatedItem(EntityViewModel viewModel)
    {
        SelectedEntity = viewModel;
    }

    public void MoveTo(EntityViewModel child, EntityViewModel? parent)
    {
        var move = new MoveEntityCommand(
            Entities,
            child.Parent,
            parent,
            child
        );
        _commandHistory.Execute(move);
    }
}