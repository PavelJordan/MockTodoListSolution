using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

/// <summary>
/// Open the dialog window from MainWindowView, whether user is sure and wait for reply by that dialog closing.
/// If true, delete all sessions.
/// </summary>
public class SessionDeletionRequest: AsyncRequestMessage<bool?>
{
}
