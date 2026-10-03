using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GameKit.ViewModels;

public partial class MoveToDialogViewModel : ObservableObject
{
    private readonly EntityViewModel _entityToMove;
    private readonly IEnumerable<EntityViewModel> _allEntities;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private EntityViewModel? _selectedEntity;

    public ObservableCollection<EntityViewModel> FilteredEntities { get; } = new();

    public MoveToDialogViewModel(EntityViewModel entityToMove, IEnumerable<EntityViewModel> allEntities)
    {
        _entityToMove = entityToMove;
        _allEntities = allEntities;
        RefreshFilter();
    }

    partial void OnSearchTextChanged(string value) => RefreshFilter();

    private void RefreshFilter()
    {
        FilteredEntities.Clear();
        var candidates = FlattenEntitiesExcludingSelf(_allEntities, _entityToMove)
            .Where(e => e.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        foreach (var candidate in candidates)
        {
            FilteredEntities.Add(candidate);
        }
    }

    private static IEnumerable<EntityViewModel> FlattenEntitiesExcludingSelf(
        IEnumerable<EntityViewModel> source, EntityViewModel excluded)
    {
        foreach (var item in source)
        {
            if (item == excluded)
            {
                continue;
            }
            
            yield return item;

            if (item.Children.Any())
            {
                foreach (var descendant in FlattenEntitiesExcludingSelf(item.Children, excluded))
                {
                    yield return descendant;
                }
            }
        }
    }
}