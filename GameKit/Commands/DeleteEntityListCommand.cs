using System.Collections.ObjectModel;
using Core.UndoRedo;
using GameKit.ViewModels;

namespace GameKit.Commands;

public class DeleteEntityListCommand : IUndoableCommand
{
    private readonly ObservableCollection<EntityViewModel> _entities;
    private readonly EntityViewModel _viewModel;
    private readonly Action<EntityViewModel> _setSelection;
    private readonly EntityViewModel _selectionAfterRemove;
    private readonly int _originalIndexPosition;
    
    public DeleteEntityListCommand(
        ObservableCollection<EntityViewModel> entities,
        EntityViewModel viewModel,
        Action<EntityViewModel> setSelection,
        EntityViewModel selectionAfterRemove)
    {
        _entities = entities;
        _viewModel = viewModel;
        _setSelection = setSelection;
        _selectionAfterRemove = selectionAfterRemove;
        _originalIndexPosition = entities.IndexOf(viewModel);
    }
    
    public void Execute()
    {
        _entities.Remove(_viewModel);
        _setSelection(_selectionAfterRemove);
        _viewModel.Parent?.Entity.RemoveChild(_viewModel.Entity);
    }

    public void Undo()
    {
        if (_viewModel.Parent is not null)
        {
            FindAndUndoEntity(_entities, _viewModel);
            _viewModel.Parent.Entity.AddChild(_viewModel.Entity, _originalIndexPosition);
        }
        else
        {
            _entities.Insert(_originalIndexPosition, _viewModel);
        }
        
        _setSelection(_viewModel);
    }

    private void FindAndUndoEntity(ObservableCollection<EntityViewModel> entities, EntityViewModel viewModel)
    {
        entities.Insert(_originalIndexPosition, viewModel);
        
    }
    
    

    public string Description => $"Remove {_viewModel.EntityType}";
}