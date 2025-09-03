using System;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using AvaloniaToDoListTrackerAndVisualizer.Services;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

/// <summary>
/// Mock with data for design time operations
/// </summary>
public class MockMainWindowViewModel: MainWindowViewModel
{
    public MockMainWindowViewModel(): base(new TaskApplicationFileService())
    {
        var groups = new GroupListViewModel(new LocalizationProvider());
        Tasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World 0!"), groups));
        Tasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World 1!"), groups));
        Tasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World 2!"), groups));
        Tasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World 3!"), groups));
        Tasks.AllTasks.Collection[2].TaskModel.Prerequisites.Collection.Add(Tasks.AllTasks.Collection[1].TaskModel);
        Tasks.AllTasks.Collection[3].TaskModel.IsCompleted = true;
        Tasks.AllTasks.Collection.Add(new TaskViewModel(new TaskModel("Hello, World 4!"), groups));
        Tasks.AllTasks.Collection[4].TaskModel.Prerequisites.Collection.Add(Tasks.AllTasks.Collection[3].TaskModel);
        Tasks.AllTasks.Collection[4].TaskModel.SoftDeadline = DateTime.Today.AddDays(1).Date;
    }
}
