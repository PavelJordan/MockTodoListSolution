using System.Collections.Generic;
using System.Collections.ObjectModel;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

public class SessionTaskSelectionRequest(TaskListViewModel allTasks, GroupListViewModel groups): AsyncRequestMessage<TaskViewModel?>
{
    public TaskListViewModel AllTasks { get; } = allTasks;
    public GroupListViewModel Groups { get; } = groups;
}

public class PrerequisiteTaskSelectionRequest(TaskViewModel task, TaskListViewModel allTasks): AsyncRequestMessage<IEnumerable<TaskModel>?>
{
    public TaskViewModel Task { get; } = task;
    public TaskListViewModel AllTasks { get; } = allTasks;
}
