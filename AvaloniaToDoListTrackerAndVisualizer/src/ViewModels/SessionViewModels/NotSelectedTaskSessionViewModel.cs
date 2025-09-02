namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class NotSelectedTaskSessionViewModel: ViewModelBase
{
    public SessionViewModel ParentSession { get; }

    public NotSelectedTaskSessionViewModel()
    {
        ParentSession = new(new (), new(new()));
    }

    public NotSelectedTaskSessionViewModel(SessionViewModel parentSession)
    {
        ParentSession = parentSession;
    }
}
