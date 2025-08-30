using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

public class OpenGroupSelectionRequest(GroupListViewModel groups): AsyncRequestMessage<Group?>
{
    public GroupListViewModel Groups { get; } = groups;
}
