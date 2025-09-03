namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

/// <summary>
/// Received by TaskEditView to close itself gracefully and return the item it was editing.
/// (this is also for saving the item).
/// </summary>
public class CloseTaskEditMessage
{
}

/// <summary>
/// Received by SubTaskEditView to close itself gracefully and.
/// Item does not need to be returned, operations are always done on registered instance.
/// </summary>
public class CloseSubTaskEditMessage
{
    
}
