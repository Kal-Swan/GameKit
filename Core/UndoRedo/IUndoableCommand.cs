namespace Core.UndoRedo;

public interface IUndoableCommand
{
    void Execute();
    void Undo();
    string Description { get; }
}