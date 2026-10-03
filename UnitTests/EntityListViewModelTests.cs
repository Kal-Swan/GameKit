using System.Collections.ObjectModel;
using Core.Entities;
using Core.UndoRedo;
using Domain.Entities;
using Domain.Entities.Tree;
using GameKit.Commands;
using GameKit.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests;

public class EntityListViewModelTests
{

    [Fact]
    public void RemoveSecondRootChild_FromMethod_FindAndRemoveEntity()
    {
        var (list, root1, root2) = BuildTwoRootTrees();
        
        var target = root2.Children.Single(c => c.Name == "Target");
        list.SelectedEntity = target;
        
        list.TryRemoveEntity(list.Entities, target);
        
        Assert.Single(root2.Children);
        Assert.Single(root1.Children);
        Assert.Equal(2, list.Entities.Count);
    }


    private static (EntityListViewModel List, EntityViewModel Root1, EntityViewModel Root2) BuildTwoRootTrees()
    {
        var services = new SetupTests().BuildProvider();

        var list = services.GetRequiredService<EntityListViewModel>();
        var factory = services.GetRequiredService<IEntityViewModelFactory>();
        
        var root1 = new ItemEntity
        {
            Name = "Root 1",
            Children = { new ItemEntity { Name = "Root 1 Child" } }
        };

        var root2 = new ItemEntity
        {
            Name = "Root 2",
            Children =
            {
                new ItemEntity { Name = "Target" },
                new ItemEntity { Name = "Sibling" }
            }
        };

        var root1ViewModel = factory.CreateFor(root1);
        var root2ViewModel = factory.CreateFor(root2);
        
        list.Entities.Add(root1ViewModel);
        list.Entities.Add(root2ViewModel);
        
        return (list, root1ViewModel, root2ViewModel);
    } 
}