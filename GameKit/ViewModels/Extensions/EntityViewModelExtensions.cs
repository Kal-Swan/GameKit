namespace GameKit.ViewModels.Extensions;

public static class EntityViewModelExtensions
{
    public static IEnumerable<EntityViewModel> Flatten(this IEnumerable<EntityViewModel> source)
    {
        foreach (var item in source)
        {
            yield return item;
        
            foreach (var descendant in item.Children.Flatten())
            {
                yield return descendant;
            }
        }
    }
}