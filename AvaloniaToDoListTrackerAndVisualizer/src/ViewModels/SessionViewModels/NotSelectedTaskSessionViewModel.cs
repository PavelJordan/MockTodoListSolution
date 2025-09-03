namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

/// <summary>
/// Wrapper for SessionViewModel - only for avalonia to know which View to use via ViewLocator
/// </summary>
public class NotSelectedTaskSessionViewModel: ViewModelBase
{
    public SessionViewModel ParentSession { get; }
    
    public NotSelectedTaskSessionViewModel(SessionViewModel parentSession)
    {
        ParentSession = parentSession;
    }
}
