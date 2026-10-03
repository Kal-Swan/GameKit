using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Entities;
using Core.UndoRedo;
using Core.Validation;
using Domain.Entities;

namespace GameKit.ViewModels;

public partial class QuestNodeViewModel : EntityViewModel
{
    private readonly IValidator<QuestEntity> _validator;
    
    [ObservableProperty] private bool _isSelected;
    [ObservableProperty] private bool _isExpanded = true;
    [ObservableProperty] private string _description;
    
    public ObservableCollection<string> DescriptionErrors { get; } = new();
    

    public QuestNodeViewModel(QuestEntity entity, IEntityViewModelFactory entityViewModelFactory, IEntityFactory entityFactory, IValidator<QuestEntity> validator, ISelectionService selectionService, ICommandHistory history) 
        : base(entity, entityViewModelFactory, entityFactory, selectionService, history)
    {
        _validator = validator;
        _description = entity.Description;
    }
    
    partial void OnDescriptionChanged(string value)
    {
        ((QuestEntity)Entity).Description = value;
        _touchedFields.Add(nameof(QuestEntity.Description));
        Validate();
    }
    
    protected override void SetPropertyValue(string propertyName, object? value)
    {
        switch (propertyName)
        {
            case nameof(Name): Name = (string)value!; break;
            case nameof(Description): Description = (string)value!; break;
            default: throw new ArgumentException($"Unknown property: {propertyName}");
        }
    }
    
    protected override object? GetPropertyValue(string propertyName)
    {
        return propertyName switch
        {
            nameof(Entity.Name) => Entity.Name,
            nameof(QuestEntity.Description) => ((QuestEntity)Entity).Description,
            _ => throw new ArgumentException($"Property '{propertyName}' is not supported.", nameof(propertyName)),
        };
    }

    protected override void Validate()
    {
        NameErrors.Clear();
        DescriptionErrors.Clear();
        var result = _validator.Validate((QuestEntity)Entity);
        foreach (var errorMessage in result.ErrorMessages)
        {
            if (!_touchedFields.Contains(errorMessage.PropertyName))
            {
                continue;
            }
            
            switch (errorMessage.PropertyName)
            {
                case nameof(QuestEntity.Name):
                    NameErrors.Add(errorMessage.Message);
                    break;
                case nameof(QuestEntity.Description):
                    DescriptionErrors.Add(errorMessage.Message);
                    break;
            }
        }
    }
}