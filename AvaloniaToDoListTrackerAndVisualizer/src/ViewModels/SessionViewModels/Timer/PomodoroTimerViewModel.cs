using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public enum TimerState
{
    Work, Break
}

public partial class PomodoroTimerViewModel: ViewModelBase
{
    public TimerViewModel Timer { get; }
    

    public PomodoroTimerViewModel(TimerViewModel timer)
    {
        Timer = timer;
    }

    [ObservableProperty] private uint _minutesWork = 20;
    [ObservableProperty] private uint _minutesShortBreak = 5;
    [ObservableProperty] private uint _shortBeforeLong = 4;
    [ObservableProperty] private uint _minutesLongBreak = 20;
    
    public TimeSpan WorkDone { get; private set; }
    
    public TimeSpan BreakDone { get; private set; }
    

    // TODO private uint _breaksUsedUp = 0;

    public void AddToBreak(TimeSpan time)
    {
        BreakDone += time;
    }
    
    public void AddToWork(TimeSpan time)
    {
        WorkDone += time;
    }
    
    public TimerState State { get; private set; } = TimerState.Work;

    [RelayCommand]
    private void SwitchWorkBreak()
    {
        bool wasRunning = !Timer.Idle;

        if (wasRunning)
        {
            Timer.Stop();
        }
        
        WorkDone = TimeSpan.Zero;
        BreakDone = TimeSpan.Zero;
        if (State == TimerState.Work)
        {
            State = TimerState.Break;
        }
        else if (State == TimerState.Break)
        {
            State = TimerState.Work;
        }
        else
        {
            throw new Exception("Unknown timer state");
        }

        if (Timer.PreviewedTask is not null && wasRunning)
        {
            Timer.Start(Timer.PreviewedTask);
        }
        else
        {
            Timer.RefreshTimerProperties();
        }
    }
}
