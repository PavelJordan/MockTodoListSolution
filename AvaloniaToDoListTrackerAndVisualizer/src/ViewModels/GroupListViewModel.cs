using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using AvaloniaToDoListTrackerAndVisualizer.Wrappers;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class GroupListViewModel(LocalizationProvider localization): ViewModelBase
{
    public LocalizationProvider Localization { get; } = localization;
    
    // TODO use GroupViewModel instead of Group
    public ObservableChildrenCollectionWrapper<Group> AllGroups { get; } = new();
}
