namespace Core.UndoRedo;

public class CommandHistory : ICommandHistory
{
    private readonly Stack<IUndoableCommand> _undoStack = new();
    private readonly Stack<IUndoableCommand> _redoStack = new();
    
    public IUndoableCommand? CurrentCommand => _undoStack.Count > 0 ? _undoStack.Peek() : null;
    
    public void Execute(IUndoableCommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear();
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Undo()
    {
        if (_undoStack.Count == 0)
        {
            return;
        }
        var command = _undoStack.Pop();
        command.Undo();
        _redoStack.Push(command);
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Redo()
    {
        if (_redoStack.Count == 0)
        {
            return;
        }
        var command = _redoStack.Pop();
        command.Execute();
        _undoStack.Push(command);
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;
    
    public event EventHandler? StateChanged;

    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public IReadOnlyCollection<IUndoableCommand> GetUndoStack()
    {
        return _undoStack;
    }

    public IReadOnlyCollection<IUndoableCommand> GetRedoStack()
    {
        return _redoStack;
    }
}