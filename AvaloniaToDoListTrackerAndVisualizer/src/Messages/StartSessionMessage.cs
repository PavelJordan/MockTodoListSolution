using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

public class StartSessionMessage(TaskListViewModel tasks, GroupListViewModel groups)
{
    public TaskListViewModel Tasks { get; } = tasks;
    public GroupListViewModel Groups { get; } = groups;
}
