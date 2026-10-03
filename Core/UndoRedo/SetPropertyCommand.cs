namespace Core.UndoRedo;

public class SetPropertyCommand<T>(string description, Action<T> setter, T oldValue, T newValue) : IUndoableCommand
{
    public void Execute() => setter(newValue);
    public void Undo() => setter(oldValue);
    public string Description { get; } = description;
}