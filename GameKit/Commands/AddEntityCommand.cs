using System.Collections.ObjectModel;
using Core.UndoRedo;
using GameKit.ViewModels;

namespace GameKit.Commands;

public class AddEntityCommand(
    ObservableCollection<EntityViewModel> entities,
    EntityViewModel viewModel,
    Action<EntityViewModel> setSelection,
    EntityViewModel previousSelection) : IUndoableCommand
{

    public void Execute()
    {
        entities.Add(viewModel);
        setSelection(viewModel);
    }

    public void Undo()
    {
        entities.Remove(viewModel);
        setSelection(previousSelection);
    }

    public string Description => $"Add {viewModel.EntityType}";
}