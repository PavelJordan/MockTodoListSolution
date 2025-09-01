using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Providers;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class MockSessionTaskSelectionViewModel: SessionTaskSelectionViewModel
{
    public MockSessionTaskSelectionViewModel() : base(new TaskListViewModel())
    {
        LocalizationProvider localization = new();
        GroupListViewModel groups = new(localization);
        // TODO task view model doesn't need localization
        AllTasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World!"), groups, localization));
        AllTasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World! 2"), groups, localization));
        AllTasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World! 3"), groups, localization));
    }
}
