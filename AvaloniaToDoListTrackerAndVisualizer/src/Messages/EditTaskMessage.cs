using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

public class EditTaskMessage(TaskViewModel taskToEdit, bool newTask, TaskListViewModel allTasks): AsyncRequestMessage<TaskViewModel?>
{
    public TaskViewModel TaskToEdit { get; } = taskToEdit;
    public bool NewTask { get; } = newTask;
    public TaskListViewModel AllTasks { get; } = allTasks;
}
