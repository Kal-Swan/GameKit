using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Entities;
using Core.UndoRedo;
using Core.Validation;
using Domain.Entities;

namespace GameKit.ViewModels;

public partial class ItemEntityViewModel : EntityViewModel
{
    private readonly IValidator<ItemEntity> _validator;
    
    public ObservableCollection<string> ValueErrors { get; } = new();
    public ObservableCollection<string> DescriptionErrors { get; } = new();
    
    [ObservableProperty] private int _value;
    [ObservableProperty] private string _description;
    
    
    public ItemEntityViewModel(ItemEntity entity, IEntityViewModelFactory entityViewModelFactory, IEntityFactory entityFactory, IValidator<ItemEntity> validator, ISelectionService selectionService, ICommandHistory commandHistory) 
        : base(entity, entityViewModelFactory, entityFactory, selectionService, commandHistory)
    {
        _validator = validator;
        _value = entity.Value;
        _description = entity.Description;
        Validate();
    }
    
    partial void OnValueChanged(int value)
    {
        _touchedFields.Add(nameof(ItemEntity.Value));
        ((ItemEntity)Entity).Value = value;
        Validate();
    }
    
    partial void OnDescriptionChanged(string value)
    {
        ((ItemEntity)Entity).Description = value;
        _touchedFields.Add(nameof(ItemEntity.Description));
        Validate();
    }

    protected override object? GetPropertyValue(string propertyName)
    {
        return propertyName switch
        {
            nameof(ItemEntity.Name) => Entity.Name,
            nameof(ItemEntity.Value) => ((ItemEntity)Entity).Value,
            nameof(ItemEntity.Description) => ((ItemEntity)Entity).Description,
            _ => throw new ArgumentException($"Property '{propertyName}' is not supported.", nameof(propertyName)),
        };
    }
    
    protected override void SetPropertyValue(string propertyName, object? value)
    {
        switch (propertyName)
        {
            case nameof(Name): Name = (string)value!; break;
            case nameof(Value): Value = (int)value; break;
            case nameof(Description): Description = (string)value!; break;
            default: throw new ArgumentException($"Unknown property: {propertyName}");
        }
    }
    
    protected override void Validate()
    {
        NameErrors.Clear();
        ValueErrors.Clear();
        DescriptionErrors.Clear();
        var result = _validator.Validate((ItemEntity)Entity);
        foreach (var errorMessage in result.ErrorMessages)
        {
            if (!_touchedFields.Contains(errorMessage.PropertyName))
            {
                continue;
            }
            
            switch (errorMessage.PropertyName)
            {
                case nameof(ItemEntity.Name):
                    NameErrors.Add(errorMessage.Message);
                    break;
                case nameof(ItemEntity.Value):
                    ValueErrors.Add(errorMessage.Message);
                    break;
                case nameof(ItemEntity.Description):
                    DescriptionErrors.Add(errorMessage.Message);
                    break;
            }
        }
    }
}