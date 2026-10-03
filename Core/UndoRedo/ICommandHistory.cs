namespace Core.UndoRedo;

public interface ICommandHistory
{
    void Execute(IUndoableCommand command);
    void Undo();
    void Redo();
    bool CanUndo { get; }
    bool CanRedo { get; }
    event EventHandler? StateChanged;
    void Clear();
    
    IUndoableCommand? CurrentCommand { get; }
    
    IReadOnlyCollection<IUndoableCommand> GetUndoStack();
    IReadOnlyCollection<IUndoableCommand> GetRedoStack();
}