using System.Collections.Generic;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

/// <summary>
/// Open dialog for selection of task to work on (if successful, it is returned, otherwise, null is returned).
/// Sent and received by Session (sent by ViewModel, received by View)
/// </summary>
/// <param name="allTasks"> Tasks to select from </param>
public class SessionTaskSelectionRequest(TaskListViewModel allTasks): AsyncRequestMessage<TaskViewModel?>
{
    public TaskListViewModel AllTasks { get; } = allTasks;
}

/// <summary>
/// Open dialog for selection of task to that are prerequisites of another task.
/// Right now, cycles and everything are enabled (they are harmless and can be later removed by hand).
/// Sent and received by TaskEdit (sent by ViewModel, received by View)
/// </summary>
/// <param name="task"> Tasks to select prerequisites for </param>
/// <param name="allTasks"> Tasks to select from </param>
public class PrerequisiteTaskSelectionRequest(TaskViewModel task, TaskListViewModel allTasks): AsyncRequestMessage<IEnumerable<TaskModel>?>
{
    public TaskViewModel Task { get; } = task;
    public TaskListViewModel AllTasks { get; } = allTasks;
}
