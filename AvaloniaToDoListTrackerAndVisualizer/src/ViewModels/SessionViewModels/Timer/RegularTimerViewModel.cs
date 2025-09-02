namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class RegularTimerViewModel: ViewModelBase
{
    public TimerViewModel Timer { get; }
    public bool OnPause { get; private set; } = false;

    public RegularTimerViewModel(TimerViewModel timer)
    {
        Timer = timer;
    }
}
