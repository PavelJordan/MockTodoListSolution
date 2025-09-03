using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

/// <summary>
/// Edit task, as called from main window. Can be either used to create new task (set by setting newTask bool),
/// or to edit old one. If newTask is set to true, the buttons change to correspond better (instead of "back" it is "save").
/// If null is returned, window was closed ungracefully - in new task version, this means discarding it, otherwise,
/// just ignore the fact and act as if it was closed gracefully.
/// </summary>
/// <param name="taskToEdit"> Task to edit (or new task with fields set to defaults)</param>
/// <param name="newTask"> Whether the task is new (for cosmetics purposes - save button instead of back)</param>
/// <param name="allTasks"> Other tasks (for prerequisites) </param>
public class EditTaskMessage(TaskViewModel taskToEdit, bool newTask, TaskListViewModel allTasks): AsyncRequestMessage<TaskViewModel?>
{
    public TaskViewModel TaskToEdit { get; } = taskToEdit;
    public bool NewTask { get; } = newTask;
    public TaskListViewModel AllTasks { get; } = allTasks;
}
