namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class PomodoroTimerViewModel: ViewModelBase
{
    public TimerViewModel Timer { get; }

    public PomodoroTimerViewModel()
    {
        Timer = new TimerViewModel();
    }

    public PomodoroTimerViewModel(TimerViewModel timer)
    {
        Timer = timer;
    }
}
