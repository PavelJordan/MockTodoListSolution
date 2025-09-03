using AvaloniaToDoListTrackerAndVisualizer.ViewModels;

namespace AvaloniaToDoListTrackerAndVisualizer.Messages;

/// <summary>
/// Sent (viewmodel) and received (view) by session window to
/// open dialog with the timer, where user can tweak its settings
/// (like set whether regular or pomodoro is used).
/// </summary>
/// <param name="timer"> The timer to modify </param>
public class SetupTimerRequest(TimerViewModel timer)
{
    public TimerViewModel Timer { get; } = timer;
}
