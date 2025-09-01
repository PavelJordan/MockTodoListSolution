using System.Collections.Generic;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

public class ClosePrerequisiteSelectionMessage(IEnumerable<TaskModel> resultPrerequisites)
{
    public IEnumerable<TaskModel> ResultPrerequisites { get; } = resultPrerequisites;
    
}
