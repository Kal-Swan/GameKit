using Core.Entities;
using Core.UndoRedo;
using Core.Validation;
using Core.Validation.ItemValidations;
using Domain.Entities;
using Domain.Entities.Tree;
using Domain.Projects;
using GameKit.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests;

public class SetupTests
{
    public ItemEntityViewModel CreateFakeItemEntityViewModel(ITreeNode entity)
    {
        var buildServiceProvider = BuildProvider();
        var commandHistory = new CommandHistory();
        var entityViewModelFactory = new EntityViewModelFactory(buildServiceProvider, commandHistory);
        return (ItemEntityViewModel)entityViewModelFactory.CreateFor(entity);
    }
    
    public EntityViewModel CreateFakeEntityViewModel(ITreeNode entity)
    {
        var buildServiceProvider = BuildProvider();
        var commandHistory = new CommandHistory();
        var entityViewModelFactory = new EntityViewModelFactory(buildServiceProvider, commandHistory);
        return entityViewModelFactory.CreateFor(entity);
    }
    
    public IEnumerable<EntityViewModel> CreateFakeEntityViewModels(IEnumerable<ITreeNode> entities)
    {
        var buildServiceProvider = BuildProvider();
        var commandHistory = new CommandHistory();
        var entityViewModelFactory = new EntityViewModelFactory(buildServiceProvider, commandHistory);
        
        var entityViewModels = new List<EntityViewModel>();

        foreach (var entity in entities)
        {
            var createdEntity = entityViewModelFactory.CreateFor(entity);
            entityViewModels.Add(createdEntity);
        }

        return entityViewModels;
    }
    
    public IServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton<EntityListViewModel>();
        services.AddSingleton<CommandHistoryViewModel>();
        services.AddSingleton<ICommandHistory, CommandHistory>();
        services.AddSingleton<IEntityFactory, EntityFactory>();
        services.AddSingleton<IEntityViewModelFactory, EntityViewModelFactory>();
        services.AddTransient<IValidator<ItemEntity>, Validator<ItemEntity>>();
        services.AddTransient<IValidator<QuestEntity>, Validator<QuestEntity>>();
        services.AddTransient<IValidationRule<ItemEntity>, ItemValueRangeRule>();
        services.AddTransient<IValidationRule<ItemEntity>, ItemNameRequiredRule>();
        services.AddTransient<IValidationRule<ItemEntity>, ItemDescriptionLengthRule>();
        services.AddSingleton<ICommandHistory, CommandHistory>();
        services.AddSingleton<IProjectRepository, JsonProjectRepository>();
        services.AddSingleton<ISelectionService>(sp => sp.GetRequiredService<EntityListViewModel>());
        return services.BuildServiceProvider();
    }
}