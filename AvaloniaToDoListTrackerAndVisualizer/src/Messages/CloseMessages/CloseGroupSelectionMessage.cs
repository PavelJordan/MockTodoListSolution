using AvaloniaToDoListTrackerAndVisualizer.Models;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;


/// <summary>
/// Close group selection with either null in GroupToSelect to
/// select no group, or something to select one.
/// Without graceful close like this, it automatically deselects
/// group.
/// </summary>
/// <param name="groupToSelect"> With instance to select, or null if deselect should happen</param>
public class CloseGroupSelectionMessage(Group? groupToSelect)
{
    public Group? GroupToSelect { get; } = groupToSelect;
}
