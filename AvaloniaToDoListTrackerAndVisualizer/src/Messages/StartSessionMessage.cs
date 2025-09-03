using AvaloniaToDoListTrackerAndVisualizer.ViewModels;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

/// <summary>
/// Called by main window to close itself and open another smaller window with session.
/// </summary>
/// <param name="tasks"> Tasks to work on </param>
/// <param name="groups"> all groups (can be later used for filtering - right now, it does nothing) </param>
public class StartSessionMessage(TaskListViewModel tasks, GroupListViewModel groups)
{
    public TaskListViewModel Tasks { get; } = tasks;
    
    // TODO implement filtering in session task selection
    public GroupListViewModel Groups { get; } = groups;
}
