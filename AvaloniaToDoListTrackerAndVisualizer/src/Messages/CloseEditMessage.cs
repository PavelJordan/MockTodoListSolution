using AvaloniaToDoListTrackerAndVisualizer.ViewModels;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

public class CloseEditMessage(TaskViewModel result)
{
    public TaskViewModel Result { get; } = result;
}
