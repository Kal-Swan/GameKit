using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.UndoRedo;

namespace GameKit.ViewModels;

public partial class CommandHistoryViewModel : ObservableObject
{
    private readonly ICommandHistory _commandHistory;
    public ObservableCollection<IUndoableCommand> UndoStack { get; set; } = [];
    public ObservableCollection<IUndoableCommand> RedoStack { get; set; } = [];

    public IUndoableCommand Current => _commandHistory.CurrentCommand!;

    public CommandHistoryViewModel(ICommandHistory commandHistory)
    {
        _commandHistory = commandHistory;
        _commandHistory.StateChanged += OnHistoryStateChanged;
    }

    public void ClearHistory()
    {
        _commandHistory.Clear();
        UndoStack.Clear();
        RedoStack.Clear();
    }

    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo()
    {
        _commandHistory.Undo();
    }

    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void Redo()
    {
        _commandHistory.Redo();
    }

    private void OnHistoryStateChanged(object? sender, EventArgs e)
    {
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
        RefreshHistory();
    }

    private void RefreshHistory()
    {
        UndoStack.Clear();
        foreach (var command in _commandHistory.GetUndoStack())
        {
            UndoStack.Add(command);
        }
        
        RedoStack.Clear();
        foreach (var command in _commandHistory.GetRedoStack())
        {
            RedoStack.Add(command);
        }
    }
    
    public bool CanUndo => _commandHistory.CanUndo;
    public bool CanRedo => _commandHistory.CanRedo;
}