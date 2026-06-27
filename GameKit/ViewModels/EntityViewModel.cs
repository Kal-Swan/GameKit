using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GameKit.Core.Features.Validation;
using GameKit.Domain.Entities;

namespace GameKit.ViewModels;

public partial class EntityViewModel : ObservableObject
{
    public IEntity Entity { get; }

    public EntityViewModel(IEntity entity)
    {
        Entity = entity;
        _name = entity.Name;
    }

    partial void OnNameChanged(string value)
    {
        Entity.Name = value;
        Validate();
    }
    
    [ObservableProperty] private string _name;

    public string EntityType => Entity.EntityType;
    public Guid Id => Entity.Id;

    protected virtual void Validate()
    {
    }
}