namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

/// <summary>
/// Wrapper for SessionViewModel - only for avalonia to know which View to use via ViewLocator
/// </summary>
public class SelectedTaskSessionViewModel: ViewModelBase
{
    public SelectedTaskSessionViewModel(SessionViewModel parentSession)
    {
        ParentSession = parentSession;
    }
    

    public SessionViewModel ParentSession { get;  }
}
