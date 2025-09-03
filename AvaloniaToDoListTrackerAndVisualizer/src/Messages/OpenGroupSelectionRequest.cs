using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

/// <summary>
/// Open group selection, as called from the task list in main window (home view).
/// Opens dialog in MainWindow. The responded group should be set to the TaskViewModel group,
/// whoever called this request. If stopped ungracefully (or the unselect button was clicked),
/// null is responded and so null group is selected.
/// </summary>
/// <param name="groups"></param>
public class OpenGroupSelectionRequest(GroupListViewModel groups): AsyncRequestMessage<Group?>
{
    public GroupListViewModel Groups { get; } = groups;
}
