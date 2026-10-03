using System.Collections.ObjectModel;
using Core.UndoRedo;
using GameKit.ViewModels;

namespace GameKit.Commands;

public class MoveEntityCommand: IUndoableCommand
{

    private ObservableCollection<EntityViewModel> RootEntities { get; set; }
    private EntityViewModel? OldParent { get; set; }
    private EntityViewModel? NewParent { get; set; }
    private EntityViewModel EntityToMove { get; set; }

    private readonly int _originalIndexPosition;

    public MoveEntityCommand(ObservableCollection<EntityViewModel> rootEntities,
        EntityViewModel? oldParent,
        EntityViewModel? newParent,
        EntityViewModel entityToMove)
    {
        RootEntities = rootEntities;
        OldParent = oldParent;
        NewParent = newParent;
        EntityToMove = entityToMove;
        _originalIndexPosition = oldParent == null ? rootEntities.IndexOf(entityToMove) : oldParent.Children.IndexOf(entityToMove);
    }
    
    public void Execute()
    {
        if (OldParent == null)
        {
            RootEntities.Remove(EntityToMove);
        }
        else
        {
            OldParent.Entity.RemoveChild(EntityToMove.Entity);
            OldParent.Children.Remove(EntityToMove);
        }

        if (NewParent == null)
        {
            RootEntities.Add(EntityToMove);
            EntityToMove.Parent = null;
        }
        else
        {
            NewParent?.Entity.AddChild(EntityToMove.Entity);
            NewParent?.Children.Add(EntityToMove);
            EntityToMove.Parent = NewParent;
        }
    }

    public void Undo()
    {
        if (OldParent == null)
        {
            RootEntities.Insert(_originalIndexPosition, EntityToMove);
            EntityToMove.Parent = null;
        }
        else
        {
            OldParent.Entity.AddChild(EntityToMove.Entity, _originalIndexPosition);
            OldParent.Children.Insert(_originalIndexPosition, EntityToMove);
            EntityToMove.Parent = OldParent;
        }

        if (NewParent == null)
        {
            RootEntities.Remove(EntityToMove);
        }
        else
        {
            NewParent?.Entity.RemoveChild(EntityToMove.Entity);
            NewParent?.Children.Remove(EntityToMove);
        }
        
    }

    public string Description => $"Move {EntityToMove.DisplayName} from {OldParent?.DisplayName ?? "No Parent"} to {NewParent?.DisplayName ?? "No Parent"}";
}