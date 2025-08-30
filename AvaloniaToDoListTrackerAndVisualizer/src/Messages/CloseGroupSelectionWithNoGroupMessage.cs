using AvaloniaToDoListTrackerAndVisualizer.Models.Items;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

public class CloseGroupSelection(Group? groupToSelect)
{
    public Group? GroupToSelect { get; } = groupToSelect;
}
