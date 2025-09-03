using AvaloniaToDoListTrackerAndVisualizer.ViewModels;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

/// <summary>
/// Request to TaskEditView to open another dialog with SubTaskEditView, where subtask can be modified.
/// The SubTaskViewModel sends this message with itself when clicked inside the
/// TasKEditView.
/// </summary>
/// <param name="taskToEdit"> The subtask to modify in the new window </param>
public class EditSubTaskMessage(SubTaskViewModel taskToEdit)
{
    public SubTaskViewModel TaskToEdit { get; } = taskToEdit;
}
