using AvaloniaToDoListTrackerAndVisualizer.ViewModels;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

/// <summary>
/// Used to close the window where user selects task to work on.
/// Null acts as cancel. Contents returned by that dialog closing.
/// </summary>
/// <param name="selectedTask"> instance for select, null for cancel</param>
public class CloseSessionTaskSelectionMessage(TaskViewModel? selectedTask)
{
    public TaskViewModel? SelectedTask { get; } = selectedTask;
}
