using AvaloniaToDoListTrackerAndVisualizer.ViewModels;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

public class CloseSessionTaskSelectionMessage(TaskViewModel? selectedTask)
{
    public TaskViewModel? SelectedTask { get; } = selectedTask;
}
