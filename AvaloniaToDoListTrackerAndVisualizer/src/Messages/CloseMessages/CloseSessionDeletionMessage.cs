namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

/// <summary>
/// Close the confirmation dialog, whether user is sure they want
/// to delete all session history. The contents are returned from dialog.
/// </summary>
/// <param name="shouldDeleteSession"> Whether user is sure </param>
public class CloseSessionDeletionMessage(bool shouldDeleteSession)
{
    public bool ShouldDeleteSession { get; } = shouldDeleteSession;
}
