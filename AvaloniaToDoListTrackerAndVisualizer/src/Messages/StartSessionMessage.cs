using AvaloniaToDoListTrackerAndVisualizer.ViewModels;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

public class StartSessionMessage(TaskListViewModel tasks, GroupListViewModel groups)
{
    public TaskListViewModel Tasks { get; } = tasks;
    public GroupListViewModel Groups { get; } = groups;
}
