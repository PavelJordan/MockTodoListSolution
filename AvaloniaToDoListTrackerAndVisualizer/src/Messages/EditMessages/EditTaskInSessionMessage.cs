using AvaloniaToDoListTrackerAndVisualizer.ViewModels;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

public class EditTaskInSessionMessage(TaskViewModel taskToEdit, TaskListViewModel allTasks)
{
    public TaskViewModel TaskToEdit { get; } = taskToEdit;
    public TaskListViewModel AllTasks { get; } = allTasks;
}
