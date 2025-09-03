using System;
using Avalonia.Threading;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
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
    private static bool FalseConstant => false;

    /// <summary>
    /// Session with work session parts - used on regular timer or pomodoro timer while working
    /// </summary>
    public Session CurrentSession { get; }
    
    /// <summary>
    /// Session with break session parts - used with pomodoro timer while on break.
    /// This is not saved later. It is used only to compute time on break.
    /// </summary>
    public Session BreakSessions { get; } = new Session();
    
    public LocalizationProvider Localization { get; }
    
    /// <summary>
    /// Whether timer is not running (not working nor on break)
    /// </summary>
    public bool Idle { get; private set; } = true;
    
    /// <summary>
    /// Whether timer is regular or pomodoro
    /// </summary>
    [ObservableProperty][NotifyPropertyChangedFor(nameof(SelectedTimer))] private TimerType _timerType = TimerType.RegularTimer;

    // ReSharper disable once UnusedParameterInPartialMethod
    /// <summary>
    /// If timer type changes, it needs to stop beforehand to record stats to pomodoro or regular timer
    /// </summary>
    /// <param name="value"></param>
    partial void OnTimerTypeChanging(TimerType value)
    {
        Stop();
    }

    public RegularTimerViewModel RegularTimerViewModel { get; }
    public  PomodoroTimerViewModel PomodoroTimerViewModel { get; }

    /// <summary>
    /// Maps regular or pomodoro timer to their actual ViewModels
    /// </summary>
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
    
    /// <summary>
    /// Not null if user is currently working (regular or pomodoro work)
    /// </summary>
    public Session.RunningSessionPart? RunningWorkSessionPart { get; private set; }
    
    /// <summary>
    /// Not null if user is currently on break (pomodoro only)
    /// </summary>
    public Session.RunningSessionPart? RunningBreakSessionPart { get; private set; }
    
    
    /// <summary>
    /// That that is being worked on. Not the displayed one (even though it is automatically set,
    /// unless user tampers with it)
    /// </summary>
    public TaskViewModel? TaskToWorkOn { get; private set; }
    
    /// <summary>
    /// Task that is previewed. Timer does not need to be running for it. If work on some task begins,
    /// this is set automatically
    /// </summary>
    public TaskViewModel? PreviewedTask { get; private set; }

    /// <summary>
    /// Timer to refresh the timer displayed (refreshing on UI thread)
    /// </summary>
    private readonly DispatcherTimer _refreshTimer;

    public string ButtonText
    {
        get
        {
            return Localization.SkipWorkText;
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

    /// <summary>
    /// Computed by break already done in pomodoro time by before session parts and current break session part
    /// </summary>
    public TimeSpan TimeLeftOnBreak
    {
        get
        {
            // TODO implement long break
            return TimeSpan.FromMinutes(PomodoroTimerViewModel.MinutesShortBreak) - (PomodoroTimerViewModel.BreakDone + (RunningBreakSessionPart?.TimeSoFar ?? TimeSpan.Zero)); 
        }
    }
    
    /// <summary>
    /// Computed by work already done in pomodoro time by before session parts and current break session part
    /// </summary>
    public TimeSpan TimeLeftOnWork
    {
        get
        {
            return TimeSpan.FromMinutes(PomodoroTimerViewModel.MinutesWork) - (PomodoroTimerViewModel.WorkDone + (RunningWorkSessionPart?.TimeSoFar ?? TimeSpan.Zero)); 
        }
    }

    /// <summary>
    /// Show time and if working / on break
    /// </summary>
    public string PomodoroInformationText
    {
        get
        {
            if (PomodoroTimerViewModel.State == TimerState.Work)
            {
                if (TimeLeftOnWork > TimeSpan.Zero)
                {
                    return Localization.WorkLeftText + ": -" + FormatTimeSpan(TimeLeftOnWork);
                }
                else
                {
                    return Localization.OvertimeText + ": +" + FormatTimeSpan(TimeLeftOnWork);
                }
            }
            else if (PomodoroTimerViewModel.State == TimerState.Break)
            {
                if (TimeLeftOnBreak > TimeSpan.Zero)
                {
                    return Localization.BreakLeftText +": -" + FormatTimeSpan(TimeLeftOnBreak);
                }
                else
                {
                    return Localization.OverBreakText + ": +" + FormatTimeSpan(TimeLeftOnBreak);
                }
            }
            else
            {
                throw new ArgumentException("Unknown timer state");
            }
        }
    }

    /// <summary>
    /// Show skip work/break text or end text
    /// </summary>
    public string PomodoroButtonText
    {
        get
        {
            if ((PomodoroTimerViewModel.State == TimerState.Work && TimeLeftOnWork <= TimeSpan.Zero)
                || PomodoroTimerViewModel.State == TimerState.Break && TimeLeftOnBreak <= TimeSpan.Zero)
            {
                return Localization.EndText;
            }
            else
            {
                if (PomodoroTimerViewModel.State == TimerState.Break)
                {
                    return Localization.SkipBreakText;
                }
                return Localization.SkipWorkText;
            }
        }
    }

    /// <summary>
    /// Show information about current session - total time
    /// </summary>
    public string SessionTimeInformationText
    {
        get
        {
            return Localization.SessionTotalText + ": " + CurrentSession.TotalSessionTime().ToString(@"hh\:mm\:ss");
        }
    }
    
    
    /// <summary>
    /// Show information about previewed task - how long user worked on it / how long is expected
    /// </summary>
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
            return Localization.NotSetText;
        }
        else
        {
            return timeSpan.Value.ToString(@"hh\:mm\:ss");
        }
    }
    

    public TimerViewModel(Session session, LocalizationProvider localization)
    {
        CurrentSession = session;
        Localization = localization;
        
        PomodoroTimerViewModel = new(this);
        RegularTimerViewModel = new(this);
        
        _refreshTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _refreshTimer.Tick += OnEveryRunningSecond;
    }

    /// <summary>
    /// Start working on task. Set it as previewed. Based on timer type and pomodoro state,
    /// decide whether work session part or break session part should be used.
    /// Start refresh timer. Use this method ONLY on UI thread
    /// </summary>
    /// <exception cref="ApplicationException"> If user starts another task before the last one is stopped </exception>
    public void Start(TaskViewModel taskToWorkOn)
    {
        if (!Idle)
        {
            throw new ApplicationException("User started to work on another task simultaneously with different task");
        }
        
        Idle = false;
        
        PreviewedTask = TaskToWorkOn = taskToWorkOn;
        
        
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

    /// <summary>
    /// Set previewed task, overriding empty/working on. This will be visible now.
    /// This is the only way to preview task before starting timer for it.
    /// </summary>
    public void SetPreviewTask(TaskViewModel taskToPreview)
    {
        PreviewedTask = taskToPreview;
        RefreshTimerProperties();
    }

    /// <summary>
    /// If timer is already idle, do nothing. Otherwise, based on timer type
    /// and pomodoro state, decide, whether work was done, or break. Add it to correct
    /// properties (task model time spent, pomodoro work/break).
    /// </summary>
    public void Stop()
    {
        if (Idle)
        {
            return;
        }


        if (PomodoroTimerViewModel.State == TimerState.Work ||  TimerType == TimerType.RegularTimer)
        {
            // We are either pomodoro work or regular timer - add worked time to Task we worked on
            var finishedSession = RunningWorkSessionPart!.End();
            TaskToWorkOn!.TaskModel.TimeSpent += finishedSession!.Value.Duration;
            if (TimerType == TimerType.PomodoroTimer)
            {
                // If on pomodoro, register the work
                PomodoroTimerViewModel.AddToWork(finishedSession.Value.Duration);
            }
        }
        else if (PomodoroTimerViewModel.State == TimerState.Break)
        {
            // We are pomodoro on break. Only add break time to pomodoro.
            var finishedSession = RunningBreakSessionPart!.End();
            PomodoroTimerViewModel.AddToBreak(finishedSession!.Value.Duration);
        }
        
        // Set all appropriate properties to null and stop refresh timers
        EnsureNoSessionIsRunning();
        RefreshTimerProperties();
    }

    /// <summary>
    /// Clear running sessions and worked-on task to null, set timer as idle and stop refresh timer.
    /// </summary>
    private void EnsureNoSessionIsRunning()
    {
        RunningWorkSessionPart = null;
        RunningBreakSessionPart = null;
        Idle = true;
        TaskToWorkOn = null;
        _refreshTimer.Stop();
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

    public string ShortBreaksBeforeLongText
    {
        get
        {
            return Localization.ShortBreaksBeforeLongText + " " + Localization.ComingSoonText;
        }
    }
    
    public string LongBreakTimeText
    {
        get
        {
            return Localization.LongBreakTimeText + " " + Localization.ComingSoonText;
        }
    }
}
