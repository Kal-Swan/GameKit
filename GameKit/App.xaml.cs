using System.IO;
using System.Windows;
using Core.Entities;
using Core.Plugins;
using Core.Plugins.Exporter;
using Core.UndoRedo;
using Core.Validation;
using Core.Validation.ItemValidations;
using Core.Validation.QuestValidations;
using Domain.Entities;
using Domain.Projects;
using GameKit.UiElementHelpers;
using GameKit.ViewModels;
using GameKit.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GameKit;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; } = null!;
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();
        
        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IEntityFactory, EntityFactory>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<EntityListViewModel>();
        services.AddSingleton<CommandHistoryViewModel>();
        services.AddSingleton<IEntityViewModelFactory, EntityViewModelFactory>();
        services.AddTransient<IValidator<ItemEntity>, Validator<ItemEntity>>();
        services.AddTransient<IValidator<QuestEntity>, Validator<QuestEntity>>();
        services.AddTransient<IValidationRule<ItemEntity>, ItemValueRangeRule>();
        services.AddTransient<IValidationRule<ItemEntity>, ItemNameRequiredRule>();
        services.AddTransient<IValidationRule<ItemEntity>, ItemDescriptionLengthRule>();
        services.AddTransient<IValidationRule<QuestEntity>, QuestDescriptionLengthRule>();
        services.AddTransient<IValidationRule<QuestEntity>, QuestNameRequiredRule>();
        services.AddTransient<IDialogService, DialogService>();
        services.AddSingleton<ICommandHistory, CommandHistory>();
        services.AddSingleton<IProjectRepository, JsonProjectRepository>();
        services.AddSingleton<ISelectionService>(serviceProvider => serviceProvider.GetRequiredService<EntityListViewModel>());
        
        var pluginsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "GameKit",
            "Plugins");
        
        var loader = new PluginLoader();
        
        services.AddSingleton<IExportStrategyRegistry>(serviceProvider =>
        {
            var registry = new ExportStrategyRegistry();
            registry.Register(new JsonExportStrategy());
            var exportStrategies = loader.LoadPlugins<IExportStrategy>(pluginsDirectory);
            foreach (var strategy in exportStrategies)
            {
                registry.Register(strategy);
            }
            return registry;
        });
        
        services.AddSingleton<MainWindow>();
    }
}