using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entities;
using Core.UndoRedo;
using Domain.Entities.Tree;
using GameKit.Commands;
using GameKit.Views;

namespace GameKit.ViewModels;

public partial class EntityViewModel : ObservableObject
{
    public ITreeNode Entity { get; set; }
    
    public EntityViewModel? Parent { get; set; }

    protected HashSet<string> _touchedFields = new();
    
    private Dictionary<string, object?> _preEditValues = new();
    
    private readonly ICommandHistory _commandHistory;
    private readonly ISelectionService _selectionService;

    public ObservableCollection<string> NameErrors { get; } = new();
    public ObservableCollection<EntityViewModel> Children { get; } = [];
    
    private readonly IEntityViewModelFactory _entityViewModelFactory;
    private readonly IEntityFactory _entityFactory;

    [ObservableProperty] private bool _isVisible = true;

    public EntityViewModel(ITreeNode entity, IEntityViewModelFactory entityViewModelFactory, IEntityFactory entityFactory, ISelectionService selectionService, ICommandHistory commandHistory)
    {
        Entity = entity;
        _name = entity.Name;
        _commandHistory = commandHistory;
        _selectionService = selectionService;
        _entityViewModelFactory = entityViewModelFactory;
        _entityFactory = entityFactory;
        
        foreach (var child in entity.Children)
        {
            var childViewModel = _entityViewModelFactory.CreateFor(child);
            childViewModel.Parent = this;
            Children.Add(childViewModel);
        }
    }

    partial void OnNameChanged(string value)
    {
        Entity.Name = value;
        _touchedFields.Add(nameof(Entity.Name));
        Validate();
    }
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    protected string _name;
    
    public string DisplayName => !string.IsNullOrWhiteSpace(Name) ? Name : EntityType;

    public string EntityType => Entity.EntityType;
    public Guid Id => Entity.Id;

    public void CommitPendingEdits()
    {
        foreach (var propertyName in _preEditValues.Keys.ToList())
        {
            CommitEdit(propertyName);
        }
    }
    
    public bool IsDescendantOf(EntityViewModel possibleAncestor)
    {
        for (var parent = Parent; parent is not null; parent = parent.Parent)
        {
            if (ReferenceEquals(parent, possibleAncestor)) return true;
        }

        return false;
    }

    public bool ApplyFilter(string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            IsVisible = true;
            foreach (var child in Children)
            {
                child.ApplyFilter(filter);
            }
            return true;
        }

        var selfMatches = DisplayName.Contains(filter, StringComparison.OrdinalIgnoreCase);

        var anyChildMatches = false;

        foreach (var child in Children)
        {
            if (child.ApplyFilter(filter))
            {
                anyChildMatches = true;
            }
        }
        
        IsVisible = selfMatches || anyChildMatches;
        return IsVisible;
    }
    
    [RelayCommand]
    private void BeginEdit(string propertyName)
    {
        _preEditValues[propertyName] = GetPropertyValue(propertyName);
    }
    
    [RelayCommand]
    private void CommitEdit(string propertyName)
    {
        if (!_preEditValues.TryGetValue(propertyName, out var oldValue))
        {
            return;
        }
        var newValue = GetPropertyValue(propertyName);
        _preEditValues.Remove(propertyName);
        if (Equals(oldValue, newValue))
        {
            return;
        }
        
        Action<object?> setter = v => SetPropertyValue(propertyName, v);
        var command = new SetPropertyCommand<object?>(
            $"Set {propertyName} from  {oldValue} to {newValue}", 
            setter, 
            oldValue, 
            newValue);
        RecordAndExecute(command);
    }
    
    private void RecordAndExecute(IUndoableCommand command)
    {
        _commandHistory.Execute(command);
    }
    
    protected virtual void SetPropertyValue(string propertyName, object? value)
    {
        switch (propertyName)
        {
            case nameof(Name): Name = (string)value!; break;
            default: throw new ArgumentException($"Unknown property: {propertyName}");
        }
    }
    
    protected virtual object? GetPropertyValue(string propertyName)
    {
        return propertyName switch
        {
            nameof(Entity.Name) => Entity.Name,
            _ => throw new ArgumentException($"Property '{propertyName}' is not supported.", nameof(propertyName)),
        };
    }

    protected virtual void Validate()
    {
    }
    
    [RelayCommand]
    protected virtual void AddChild(string childType)
    {
        var childEntity = _entityFactory.Create(childType);
        var childViewModel = _entityViewModelFactory.CreateFor(childEntity);

        var previousSelection = _selectionService.Current;
        var command = new AddChildEntityCommand(
            this, 
            childViewModel,
            setSelection => _selectionService.Select(setSelection),
            previousSelection);
        _commandHistory.Execute(command);
    }
    
    [RelayCommand]
    protected virtual void Delete()
    {
        var command = new DeleteEntityCommand(Parent!, this);
        _commandHistory.Execute(command);
    }
    
    [RelayCommand]
    private void OpenMoveDialog()
    {
        var dialogMoveViewModel = new MoveToDialogViewModel(this, _selectionService.AllRootEntities);
        var moveToDialog = new MoveToDialog { DataContext = dialogMoveViewModel };
        var result = moveToDialog.ShowDialog();

        if (result.GetValueOrDefault() && dialogMoveViewModel.SelectedEntity != Parent && dialogMoveViewModel.SelectedEntity != null)
        {
            var moveEntity = new MoveEntityCommand(_selectionService.AllRootEntities, Parent, dialogMoveViewModel.SelectedEntity, this);
            _commandHistory.Execute(moveEntity);
        }
    }

    [RelayCommand]
    private void ConvertToParent()
    {
        var moveEntity = new MoveEntityCommand(_selectionService.AllRootEntities, Parent, null, this);
        _commandHistory.Execute(moveEntity);
    }
}