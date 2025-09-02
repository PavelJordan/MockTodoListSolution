namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class RegularTimerViewModel: ViewModelBase
{
    public TimerViewModel Timer { get; } 

    public RegularTimerViewModel()
    {
        Timer = new TimerViewModel();
    }

    public RegularTimerViewModel(TimerViewModel timer)
    {
        Timer = timer;
    }

}
