using AvaloniaToDoListTrackerAndVisualizer.ViewModels;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

/// <summary>
/// Request to delete task from main list.
/// Literally anyone can call this and each instance of
/// TaskListViewModel deletes that task.
/// It also disposes it, so the task is of no good no more.
/// </summary>
/// <param name="taskToDelete"> TaskViewModel to delete from all TaskLists and dispose </param>
public class DeleteTaskViewModelRequest(TaskViewModel taskToDelete)
{
    public TaskViewModel TaskToDelete { get; } = taskToDelete;
}
