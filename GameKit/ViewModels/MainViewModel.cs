using CommunityToolkit.Mvvm.ComponentModel;

namespace GameKit.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public EntityListViewModel EntityList { get; }
    
    [ObservableProperty]
    private string _title = "GameKit";
    
    [ObservableProperty]
    private string _statusMessage = "Ready";

    public MainViewModel(EntityListViewModel entityList)
    {
        EntityList = entityList;
    }
}