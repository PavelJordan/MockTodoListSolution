namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class SelectedTaskSessionViewModel: ViewModelBase
{
    public SelectedTaskSessionViewModel(SessionViewModel parentSession)
    {
        ParentSession = parentSession;
    }
    

    public SessionViewModel ParentSession { get;  }
}
