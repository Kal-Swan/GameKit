using Core.UndoRedo;
using GameKit.ViewModels;

namespace GameKit.Commands;

public class AddChildEntityCommand(
    EntityViewModel parentViewModel,
    EntityViewModel childViewModel,
    Action<EntityViewModel> setSelection,
    EntityViewModel previousSelection)
    : IUndoableCommand
{
    public void Execute()
    {
        parentViewModel.Entity.AddChild(childViewModel.Entity);
        parentViewModel.Children.Add(childViewModel);
        childViewModel.Parent = parentViewModel;
        setSelection(childViewModel);
    }

    public void Undo()
    {
        parentViewModel.Entity.RemoveChild(childViewModel.Entity);
        parentViewModel.Children.Remove(childViewModel);
        childViewModel.Parent = null;
        setSelection(previousSelection);
    }

    public string Description => $"Add {childViewModel.EntityType} to {parentViewModel.Name}";
}