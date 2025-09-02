namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class NotSelectedTaskSessionViewModel: ViewModelBase
{
    public SessionViewModel ParentSession { get; }
    

    public NotSelectedTaskSessionViewModel(SessionViewModel parentSession)
    {
        ParentSession = parentSession;
    }
}
