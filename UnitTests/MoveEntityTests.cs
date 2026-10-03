using System.Collections.ObjectModel;
using Core.Entities;
using Domain.Entities;
using Domain.Entities.Tree;
using GameKit.Commands;
using GameKit.ViewModels;

namespace UnitTests;

public class MoveEntityTests
{
    [Fact]
    public void MoveRootItemToQuest()
    {
        var entityViewModels = new List<ITreeNode>
        {
            new ItemEntity { Name = "Item" },
            new QuestEntity { Name = "Quest" }
        };
        
        var setupTests = new SetupTests();
        var createdEntities = setupTests.CreateFakeEntityViewModels(entityViewModels);
        
        var entityCollection = new ObservableCollection<EntityViewModel>(createdEntities);
        
        var newParent = createdEntities.First(e => e.EntityType == "Quest");
        var itemToMove = createdEntities.First(e => e.EntityType == "Item");
        
        var moveEntity = new MoveEntityCommand(entityCollection, null, newParent, itemToMove);
        moveEntity.Execute();
        
        Assert.Contains(itemToMove, newParent.Children);
        Assert.True(entityCollection.Count == 1);
        Assert.DoesNotContain(itemToMove, entityCollection);
    }
    
    [Fact]
    public void MoveChildItemToQuest()
    {
        var setupTests = new SetupTests();

        var itemParent = setupTests.CreateFakeEntityViewModel(new ItemEntity { Name = "Item" });
        var itemChild = setupTests.CreateFakeEntityViewModel(new ItemEntity { Name = "Child Item" });
        var questEntity = setupTests.CreateFakeEntityViewModel(new ItemEntity { Name = "Quest" });
        
        itemParent.Children.Add(itemChild);

        var entityCollection = new ObservableCollection<EntityViewModel>
        {
            itemParent,
            questEntity
        };
        
        var moveEntity = new MoveEntityCommand(entityCollection, itemParent, questEntity, itemChild);
        moveEntity.Execute();
        
        Assert.Contains(itemChild, questEntity.Children);
        Assert.True(entityCollection.Count == 2);
        Assert.DoesNotContain(itemChild, itemParent.Children);
    }
}