using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

public class EditTaskMessage(TaskViewModel taskToEdit): AsyncRequestMessage<TaskViewModel?>
{
    public TaskViewModel TaskToEdit { get; } = taskToEdit;
}
