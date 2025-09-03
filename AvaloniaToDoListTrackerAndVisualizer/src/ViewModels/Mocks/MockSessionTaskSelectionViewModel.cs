using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Providers;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class MockSessionTaskSelectionViewModel: SessionTaskSelectionViewModel
{
    public MockSessionTaskSelectionViewModel() : base(new TaskListViewModel(new LocalizationProvider()))
    {
        GroupListViewModel groups = new(new());
        AllTasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World!"), groups));
        AllTasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World! 2"), groups));
        AllTasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World! 3"), groups));
    }
}
