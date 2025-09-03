using System.Collections.Generic;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

/// <summary>
/// Close prerequisites select window with enumerable of the new prerequisites.
/// These replace the old ones. Without graceful closing like this, nothing happens.
/// </summary>
/// <param name="resultPrerequisites"> New prerequisites which replace the old ones</param>
public class ClosePrerequisiteSelectionMessage(IEnumerable<TaskModel> resultPrerequisites)
{
    public IEnumerable<TaskModel> ResultPrerequisites { get; } = resultPrerequisites;
}
