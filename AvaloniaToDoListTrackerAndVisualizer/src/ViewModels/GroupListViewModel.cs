using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Wrappers;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class GroupListViewModel: ViewModelBase
{
    public ObservableChildrenCollectionWrapper<Group> AllGroups { get; } = new();
}
