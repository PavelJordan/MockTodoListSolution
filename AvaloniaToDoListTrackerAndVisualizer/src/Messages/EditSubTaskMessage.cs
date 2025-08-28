using AvaloniaToDoListTrackerAndVisualizer.ViewModels;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

public class EditSubTaskMessage(SubTaskViewModel taskToEdit)
{
    public SubTaskViewModel TaskToEdit { get; } = taskToEdit;
}
