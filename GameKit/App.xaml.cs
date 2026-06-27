using System.Configuration;
using System.Data;
using System.Windows;
using GameKit.Core.Features.Entities;
using GameKit.Core.Features.Validation;
using GameKit.Domain.Entities;
using GameKit.ViewModels;
using GameKit.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GameKit;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
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
        services.AddSingleton<IEntityViewModelFactory, EntityViewModelFactory>();
        services.AddTransient<IValidator<ItemEntity>, Validator<ItemEntity>>();
        
        services.AddSingleton<MainWindow>();
    }
}