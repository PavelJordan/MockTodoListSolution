using System;
using Avalonia.Threading;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;



public enum TimerType
{
    RegularTimer, PomodoroTimer
}

public partial class TimerViewModel: ViewModelBase, IDisposable
{
    private static bool FalseConstant { get; } = false;
    
    public Session CurrentSession { get; }
    
    public Session BreakSessions { get; } = new Session();
    
    
    public bool Idle { get; private set; } = true;
    
    [ObservableProperty][NotifyPropertyChangedFor(nameof(SelectedTimer))] private TimerType _timerType = TimerType.RegularTimer;

    partial void OnTimerTypeChanging(TimerType value)
    {
        Stop();
    }

    public RegularTimerViewModel RegularTimerViewModel { get; }
    public  PomodoroTimerViewModel PomodoroTimerViewModel { get; }

    public ViewModelBase SelectedTimer
    {
        get
        {
            if (TimerType == TimerType.RegularTimer)
            {
                return RegularTimerViewModel;
            }
            return PomodoroTimerViewModel;
        }
    }
    
    public Session.RunningSessionPart? RunningWorkSessionPart { get; private set; }
    public Session.RunningSessionPart? RunningBreakSessionPart { get; private set; }
    
    public TaskViewModel? TaskToWorkOn { get; private set; }
    public TaskViewModel? PreviewedTask { get; private set; }

    private readonly DispatcherTimer _refreshTimer;

    public string ButtonText
    {
        get
        {
            return "Skip work";
        }
    }


    public TimeSpan TimeOnTaskSoFar
    {
        get
        {
            return (PreviewedTask?.TaskModel.TimeSpent ?? TimeSpan.Zero) +
                   (RunningWorkSessionPart?.TimeSoFar ?? TimeSpan.Zero);
        }
    }

    public TimeSpan TimeLeftOnBreak
    {
        get
        {
            // TODO what if on long break?
            return TimeSpan.FromMinutes(PomodoroTimerViewModel.MinutesShortBreak) - (PomodoroTimerViewModel.BreakDone + (RunningBreakSessionPart?.TimeSoFar ?? TimeSpan.Zero)); 
        }
    }
    
    public TimeSpan TimeLeftOnWork
    {
        get
        {
            return TimeSpan.FromMinutes(PomodoroTimerViewModel.MinutesWork) - (PomodoroTimerViewModel.WorkDone + (RunningWorkSessionPart?.TimeSoFar ?? TimeSpan.Zero)); 
        }
    }

    public string PomodoroInformationText
    {
        get
        {
            if (PomodoroTimerViewModel.State == TimerState.Work)
            {
                if (TimeLeftOnWork > TimeSpan.Zero)
                {
                    return "Work left: -" + FormatTimeSpan(TimeLeftOnWork);
                }
                else
                {
                    return "Overtime: +" + FormatTimeSpan(TimeLeftOnWork);
                }
            }
            else if (PomodoroTimerViewModel.State == TimerState.Break)
            {
                if (TimeLeftOnBreak > TimeSpan.Zero)
                {
                    return "Break left: -" + FormatTimeSpan(TimeLeftOnBreak);
                }
                else
                {
                    return "Over break: +" + FormatTimeSpan(TimeLeftOnBreak);
                }
            }
            else
            {
                throw new ArgumentException("Unknown timer state");
            }
        }
    }

    public string PomodoroButtonText
    {
        get
        {
            if ((PomodoroTimerViewModel.State == TimerState.Work && TimeLeftOnWork <= TimeSpan.Zero)
                || PomodoroTimerViewModel.State == TimerState.Break && TimeLeftOnBreak <= TimeSpan.Zero)
            {
                return "end";
            }
            else
            {
                return "skip";
            }
        }
    }

    public string SessionTimeInformationText
    {
        get
        {
            return "Session total: " + CurrentSession.TotalSessionTime().ToString(@"hh\:mm\:ss");
        }
    }
    
    public string TaskTimeInformationText
    {
        get
        {
            return FormatTimeSpan(TimeOnTaskSoFar) + " / " +  FormatTimeSpan(PreviewedTask?.TaskModel.TimeExpected);
        }
    }
    
    private string FormatTimeSpan(TimeSpan? timeSpan)
    {
        if (timeSpan is null)
        {
            return "Not set"; //Localization.NotSetText;
        }
        else
        {
            return timeSpan.Value.ToString(@"hh\:mm\:ss");
        }
    }
    

    public TimerViewModel(Session session)
    {
        CurrentSession = session;
        
        PomodoroTimerViewModel = new(this);
        RegularTimerViewModel = new(this);
        
        _refreshTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _refreshTimer.Tick += OnEveryRunningSecond;
    }

    public void Start(TaskViewModel taskToWorkOn)
    {
        PreviewedTask = TaskToWorkOn = taskToWorkOn;
        Idle = false;
        
        if (PomodoroTimerViewModel.State == TimerState.Work || TimerType == TimerType.RegularTimer)
        {
            RunningWorkSessionPart = CurrentSession.Start();
        }
        else
        {
            RunningBreakSessionPart = BreakSessions.Start();
        }
        
        _refreshTimer.Start();
        RefreshTimerProperties();
    }

    public void SetPreviewTask(TaskViewModel taskToPreview)
    {
        PreviewedTask = taskToPreview;
        RefreshTimerProperties();
    }

    public void Stop()
    {
        if (Idle)
        {
            return;
        }


        if (PomodoroTimerViewModel.State == TimerState.Work ||  TimerType == TimerType.RegularTimer)
        {
            var finishedSession = RunningWorkSessionPart!.End();
            RunningWorkSessionPart = null;
            TaskToWorkOn!.TaskModel.TimeSpent += finishedSession!.Value.Duration;
            if (TimerType == TimerType.PomodoroTimer)
            {
                PomodoroTimerViewModel.AddToWork(finishedSession!.Value.Duration);
            }
        }
        else if (PomodoroTimerViewModel.State == TimerState.Break)
        {
            var finishedSession = RunningBreakSessionPart!.End();
            RunningBreakSessionPart = null;
            PomodoroTimerViewModel.AddToBreak(finishedSession!.Value.Duration);
        }
        
        Idle = true;
        TaskToWorkOn = null;
        _refreshTimer.Stop();
        RefreshTimerProperties();
    }

    private void OnEveryRunningSecond(object? sender, EventArgs e)
    {
        RefreshTimerProperties();
    }
    
    public void RefreshTimerProperties()
    {
        OnPropertyChanged(nameof(SessionTimeInformationText));
        OnPropertyChanged(nameof(TaskTimeInformationText));
        OnPropertyChanged(nameof(SelectedTimer));
        OnPropertyChanged(nameof(PomodoroInformationText));
        OnPropertyChanged(nameof(PomodoroButtonText));
    }

    public void Dispose()
    {
        _refreshTimer.Tick -= OnEveryRunningSecond;
        _refreshTimer.Stop();
    }

    [RelayCommand]
    private void CloseTimerSelectionDialog()
    {
        WeakReferenceMessenger.Default.Send(new CloseTimerSelectionDialogMessage());
    }
    
    [RelayCommand(CanExecute = nameof(FalseConstant))]
    private void NotificationSound()
    {
        // TODO
    }
}
