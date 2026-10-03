using System.IO;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Plugins.Exporter;
using Core.Tree.Extensions;
using Core.Tree.Visitors;
using Core.UndoRedo;
using Domain.Entities.Tree;
using Domain.Projects;
using GameKit.UiElementHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace GameKit.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public bool IsModified => !ReferenceEquals(History.Current, _savedAt);

    private readonly IExportStrategyRegistry _exportStrategyRegistry;
    private readonly IDialogService _dialogService;
    private readonly DispatcherTimer _statusTimer;
    
    private IServiceProvider _serviceProvider;
    private Project? _currentProject;
    
    public IEnumerable<IExportStrategy> ExportStrategies => _exportStrategyRegistry.AvailableStrategies;
    public EntityListViewModel Entities { get; }
    
    public CommandHistoryViewModel History { get; }
    
    [ObservableProperty] 
    private string _savingStatus = string.Empty;
    
    [ObservableProperty]
    private string _title = "GameKit";
    
    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty] private string _search = "";

    [ObservableProperty] 
    private bool _isBusy;

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(HistoryColumnWidth))]
    private bool _historyVisible = true;
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(TreeColumnWidth))]
    private bool _treeVisible = true;

    private IUndoableCommand? _savedAt;
    private void MarkSaved() => _savedAt = History.Current;  
    
    public GridLength HistoryColumnWidth => 
        HistoryVisible ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
    
    public GridLength TreeColumnWidth => 
        TreeVisible ? new GridLength(1, GridUnitType.Star) : new GridLength(0);

    public MainViewModel(EntityListViewModel entities, CommandHistoryViewModel history, IServiceProvider serviceProvider, IExportStrategyRegistry exportStrategyRegistry, IDialogService dialogService)
    {
        Entities = entities;
        History = history;
        _serviceProvider = serviceProvider;
        _exportStrategyRegistry = exportStrategyRegistry;
        _dialogService = dialogService;

        _statusTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
        _statusTimer.Tick += (_, _) =>
        {
            _statusTimer.Stop();
            StatusMessage = string.Empty;
        };
    }

    [RelayCommand]
    private void ValidateAll()
    {
        try
        {
            var visitor = new ValidationNodeAction(_serviceProvider);
            foreach (var entity in Entities.Entities)
            {
                if (entity.Entity is { } root)
                {
                    root.Traverse(visitor);
                }
            }

            if (visitor.Errors.Any())
            {
                _dialogService.MessageBox(string.Join(Environment.NewLine, visitor.Errors), "Validation Errors",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _dialogService.MessageBox("All entities valid");
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void ShowStats()
    {
        try
        {
            var visitor = new CountNodeAction();
            foreach (var entity in Entities.Entities)
            {
                if (entity.Entity is { } root)
                {
                    root.Traverse(visitor);
                }
            }

            var summary = string.Join(Environment.NewLine,
                visitor.CountByType.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
            _dialogService.MessageBox($"Total {visitor.TotalCount}\n\n{summary}");
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task SaveAs(string format)
    {
        Entities.CommitPendingEdits();
        
        await RunStatusTimer(
            "Saving new project...",
            "Failed to save project",
            async () =>
            {
                
                var saveFileDialog = new SaveFileDialog();
                if (!saveFileDialog.ShowDialog().GetValueOrDefault())
                {
                    return;
                }
        
                var exportStrategy = ExportStrategies.First(strategy => strategy.FormatName == format);

                if (string.IsNullOrWhiteSpace(saveFileDialog.FileName))
                {
                    _dialogService.MessageBox("File path does not exist");
                    return;
                }
                _currentProject = new Project
                {
                    Name = saveFileDialog.SafeFileName,
                    Entities = Entities.Entities.Select(e => e.Entity).ToList(),
                    Metadata = new Metadata
                    {
                        Format = format,
                        FilePath = saveFileDialog.FileName
                    }
                };
                await exportStrategy.ExportAsync(_currentProject, saveFileDialog.FileName);
                MarkSaved();
                SetStatus($"Saved {_currentProject?.Name}");
            }
        );
    }
    
    [RelayCommand]
    private async Task LoadFile()
    {
        Entities.CommitPendingEdits();
        
        if (!DiscardChangesIfAny())
        {
            return;
        }
        
        await RunStatusTimer(
            "Loading...",
            "Failed to load project",
            async () =>
        {
            var openFileDialog = new OpenFileDialog();
            if (!openFileDialog.ShowDialog().GetValueOrDefault())
            {
                return;
            }

            var extension = Path.GetExtension(openFileDialog.FileName);
            var exportStrategy = ExportStrategies.FirstOrDefault(strategy => strategy.FileExtension.ToLowerInvariant() == extension);

            if (exportStrategy is null)
            {
                _dialogService.MessageBox($"No plugin registered for file of type {extension}");
                return;
            }

            var loadedProject = await exportStrategy.ImportAsync(openFileDialog.FileName);

            if (loadedProject.Entities.Count == 0)
            {
                return;
            }
            
            Clear();

            var factory = _serviceProvider.GetRequiredService<IEntityViewModelFactory>();

            foreach (var entity in loadedProject.Entities)
            {
                Entities.Entities.Add(factory.CreateFor(entity));
            }

            _currentProject = new Project
            {
                Name = loadedProject.Name,
                Entities = loadedProject.Entities,
                Metadata = new Metadata
                {
                    Format = loadedProject.Metadata.Format,
                    FilePath = loadedProject.Metadata.FilePath
                }
            };
            
        });
    }

    [RelayCommand]
    private void NewProject()
    {
        Entities.CommitPendingEdits();
        
        if (!DiscardChangesIfAny())
        {
            return;
        }

        Clear();
    }

    [RelayCommand]
    private async Task Save()
    {
        Entities.CommitPendingEdits();
        await RunStatusTimer(
            $"Saving {_currentProject?.Name}...",
            "Failed to save project",
             async () =>
            {
                if (_currentProject is null)
                {
                    _dialogService.MessageBox("Current file path does not exist.");
                    return;
                }
                
                _currentProject.Entities = Entities.Entities.Select(e => e.Entity).ToList();

                var exportStrategy =
                    ExportStrategies.First(strategy => strategy.FormatName == _currentProject.Metadata.Format);

                await exportStrategy.ExportAsync(_currentProject, _currentProject.Metadata.FilePath);
                MarkSaved();
            });
    }

    [RelayCommand]
    private void Exit()
    {
        Application.Current.MainWindow?.Close();
    }

    private void SetStatus(string message, bool autoReset = true)
    {
        StatusMessage = message;
        _statusTimer.Stop();
        if (autoReset)
        {
            _statusTimer.Start();
        }
    }

    private async Task RunStatusTimer(string initialStatusMessage, string failedStatusMessage, Func<Task> operation)
    {
        IsBusy  = true;
        SetStatus(initialStatusMessage);
        try
        {
            await operation();
        }
        catch (Exception exception)
        {
            SetStatus(failedStatusMessage, false);
            _dialogService.MessageBox($"Something went wrong: {exception.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public bool DiscardChangesIfAny()
    {
        if (IsModified)
        {
            var messageBoxResult = _dialogService.MessageBox(
                "You have unsaved changes. Are you sure you want to continue?", "Unsaved Changes",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (messageBoxResult == MessageBoxResult.No)
            {
                return false;
            }
        }

        return true;
    }

    private void Clear()
    {
        Entities.Entities.Clear();
        Entities.SelectedEntity = null;
        History.ClearHistory();
        _currentProject = null;
        Search = string.Empty;
        _savedAt = null;
    }
    

    partial void OnSearchChanged(string value)
    {
        foreach (var root in Entities.Entities)
        {
            root.ApplyFilter(value);
        }
    }
}