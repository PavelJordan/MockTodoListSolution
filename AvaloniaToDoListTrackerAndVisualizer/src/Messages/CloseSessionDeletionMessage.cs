namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

public class CloseSessionDeletionMessage(bool shouldDeleteSession)
{
    public bool ShouldDeleteSession { get; } = shouldDeleteSession;
}
