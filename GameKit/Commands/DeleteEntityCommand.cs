using Core.UndoRedo;
using GameKit.ViewModels;

namespace GameKit.Commands;

public class DeleteEntityCommand(
    EntityViewModel parentViewModel,
    EntityViewModel childViewModel)
    : IUndoableCommand
{
    private readonly int _originalIndexPosition = parentViewModel.Children.IndexOf(childViewModel);

    public void Execute()
    {
        parentViewModel.Entity.RemoveChild(childViewModel.Entity);
        parentViewModel.Children.Remove(childViewModel);
        childViewModel.Parent = null;
    }

    public void Undo()
    {
        parentViewModel.Entity.AddChild(childViewModel.Entity);
        parentViewModel.Children.Insert(_originalIndexPosition, childViewModel);
        childViewModel.Parent = parentViewModel;
    }

    public string Description => $"Delete {childViewModel.EntityType} from {parentViewModel.Name}";
}