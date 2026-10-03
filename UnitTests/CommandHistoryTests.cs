using System.Collections.ObjectModel;
using Core.UndoRedo;
using Domain.Entities;
using Domain.Entities.Tree;
using GameKit.Commands;
using GameKit.ViewModels;

namespace UnitTests;

public class CommandHistoryTests
{
    [Fact]
    public void UndoTest()
    {
        var commandHistory = new CommandHistory();
        var testCommand = new TestCommand();
        commandHistory.Execute(testCommand);
        commandHistory.Undo();
        
        Assert.True(commandHistory.GetUndoStack().Count == 0);
        Assert.True(commandHistory.GetRedoStack().Count == 1);
        Assert.True(commandHistory.CanRedo);
    }
    
    [Fact]
    public void RedoTest()
    {
        var commandHistory = new CommandHistory();
        var testCommand = new TestCommand();
        commandHistory.Execute(testCommand);
        commandHistory.Undo();
        commandHistory.Redo();
        
        Assert.True(commandHistory.GetUndoStack().Count == 1);
        Assert.True(commandHistory.GetRedoStack().Count == 0);
        Assert.True(commandHistory.CanUndo);
        Assert.False(commandHistory.CanRedo);
    }

    [Fact]
    public void ExecuteAddEntityCommand()
    {
        var entites = new ObservableCollection<EntityViewModel>();
        var entity = new ItemEntity { Name = "Test Item" };
        var setupTests = new SetupTests();
        var newEntity = setupTests.CreateFakeItemEntityViewModel(entity);

        EntityViewModel? selected = null;

        var addEntityCommand = new AddEntityCommand(
            entites,
            newEntity,
            setSelection: vm => selected = vm,
            selected
            );
        
        addEntityCommand.Execute();
        
        Assert.True(entites.Count == 1);
        Assert.True(selected.Name == "Test Item");
    }
    
    [Fact]
    public void UndoAddEntityCommand()
    {
        var entites = new ObservableCollection<EntityViewModel>();
        var entity = new ItemEntity { Name = "Test Item" };
        var setupTests = new SetupTests();
        var newEntity = setupTests.CreateFakeItemEntityViewModel(entity);

        EntityViewModel? selected = null;

        var addEntityCommand = new AddEntityCommand(
            entites,
            newEntity,
            setSelection: vm => selected = vm,
            selected
        );
        
        addEntityCommand.Execute();
        addEntityCommand.Undo();
        
        Assert.True(entites.Count == 0);
        Assert.True(selected?.Name == null);
        Assert.True(selected == null);
    }
    
    private class TestCommand : IUndoableCommand
    {
        private string TestName { get; set; } = string.Empty;
        
        public void Execute()
        {
            TestName = "Test";
        }

        public void Undo()
        {
            TestName = string.Empty;
        }

        public string Description { get; }
    }
}