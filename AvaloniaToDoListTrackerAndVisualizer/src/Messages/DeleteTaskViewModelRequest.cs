using AvaloniaToDoListTrackerAndVisualizer.ViewModels;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

public class DeleteTaskViewModelRequest(TaskViewModel taskToDelete)
{
    public TaskViewModel TaskToDelete { get; } = taskToDelete;
}
