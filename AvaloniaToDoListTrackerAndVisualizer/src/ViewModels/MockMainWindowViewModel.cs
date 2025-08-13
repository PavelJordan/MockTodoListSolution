using AvaloniaToDoListTrackerAndVisualizer.Models.Items;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

/// <summary>
/// Mock with data for design time operations
/// </summary>
public class MockMainWindowViewModel: MainWindowViewModel
{
    public MockMainWindowViewModel()
    {
        Tasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World 0!")));
        Tasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World 1!")));
        Tasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World 2!")));
        Tasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World 3!")));
        Tasks.AllTasks.Collection[2].TaskModel.Prerequisites.Collection.Add(Tasks.AllTasks.Collection[1].TaskModel);
        Tasks.AllTasks.Collection[3].TaskModel.IsCompleted = true;
        Tasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World 4!")));
        Tasks.AllTasks.Collection[4].TaskModel.Prerequisites.Collection.Add(Tasks.AllTasks.Collection[3].TaskModel);
    }
}
