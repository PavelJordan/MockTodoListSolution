using AvaloniaToDoListTrackerAndVisualizer.ViewModels;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

public class SetupTimerRequest(TimerViewModel timer)
{
    public TimerViewModel Timer { get; } = timer;
}
