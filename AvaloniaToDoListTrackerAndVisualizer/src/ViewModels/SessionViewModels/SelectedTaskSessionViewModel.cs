namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class SelectedTaskSessionViewModel: ViewModelBase
{
    public SelectedTaskSessionViewModel(SessionViewModel parentSession)
    {
        ParentSession = parentSession;
    }

    public SelectedTaskSessionViewModel()
    {
        ParentSession = new(new(), new(new()));
    }

    public SessionViewModel ParentSession { get;  }
}
