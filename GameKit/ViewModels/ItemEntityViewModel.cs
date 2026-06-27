using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GameKit.Core.Features.Validation;
using GameKit.Domain.Entities;

namespace GameKit.ViewModels;

public partial class ItemEntityViewModel : EntityViewModel
{
    private readonly IValidator<ItemEntity> _validator;

    public ObservableCollection<string> ValidationErrors { get; } = new();
    public ItemEntityViewModel(ItemEntity entity, IValidator<ItemEntity> validator) : base(entity)
    {
        _validator = validator;
        _value = entity.Value;
        _description = entity.Description;
        Validate();
    }
    
    partial void OnValueChanged(int value)
    {
        ((ItemEntity)Entity).Value = value;
        Validate();
    }
    
    partial void OnDescriptionChanged(string value)
    {
        ((ItemEntity)Entity).Description = value;
        Validate();
    }
    
    [ObservableProperty] private int _value;
    [ObservableProperty] private string _description;
    
    protected override void Validate()
    {
        ValidationErrors.Clear();
        var result = _validator.Validate((ItemEntity)Entity);
        foreach (var errorMessage in result.ErrorMessages)
        {
            ValidationErrors.Add(errorMessage);
        }
    }
    
}