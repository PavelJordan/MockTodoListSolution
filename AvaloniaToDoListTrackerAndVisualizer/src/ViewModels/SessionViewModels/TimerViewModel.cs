using System;
using Avalonia.Controls;
using Avalonia.Threading;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Views.SessionViews.Timer;
using DynamicData.Kernel;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public enum TimerState
{
    Idle, Work, Break
}

public enum TimerType
{
    RegularTimer, PomodoroTimer
}

public class TimerViewModel: ViewModelBase, IDisposable
{
    public Session CurrentSession { get; } = new Session();
    
    public TimeWithButton TimeWithButton { get; }
    public TimeWithoutButton TimeWithoutButton { get; }
    
    public TimerState State { get; private set; } = TimerState.Idle;
    
    public TimerType TimerType { get; private set; } = TimerType.RegularTimer;
    
    public Session.RunningSessionPart? RunningSessionPart { get; private set; }
    
    
    public TaskViewModel? TaskToWorkOn { get; private set; }
    public TaskViewModel? PreviewedTask { get; private set; }
    
    
    public UserControl TimeStyle { get; }

    private readonly DispatcherTimer _refreshTimer;

    public string ButtonText
    {
        get
        {
            return "Skip work";
        }
    }

    public string SessionTimeInformationText
    {
        get
        {
            return CurrentSession.TotalSessionTime().ToString(@"hh\:mm\:ss");
        }
    }

    public TimeSpan TimeOnTaskSoFar
    {
        get
        {
            return (PreviewedTask?.TaskModel.TimeSpent ?? TimeSpan.Zero) +
                   (RunningSessionPart?.TimeSoFar ?? TimeSpan.Zero);
        }
    }

    public string TaskTimeInformationText
    {
        get
        {
            return formatTimeSpan(TimeOnTaskSoFar) + " / " +  formatTimeSpan(PreviewedTask?.TaskModel.TimeExpected);
        }
    }
    
    private string formatTimeSpan(TimeSpan? timeSpan)
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
    

    public TimerViewModel()
    {
        TimeWithButton = new TimeWithButton(this);
        TimeWithoutButton = new TimeWithoutButton(this);
        TimeStyle = TimeWithoutButton;

        _refreshTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _refreshTimer.Tick += OnEveryRunningSecond;
    }

    public void Start(TaskViewModel taskToWorkOn)
    {
        PreviewedTask = TaskToWorkOn = taskToWorkOn;
        State = TimerState.Work;
        RunningSessionPart = CurrentSession.Start();
        _refreshTimer.Start();
        OnPropertyChanged(nameof(SessionTimeInformationText));
        OnPropertyChanged(nameof(TaskTimeInformationText));
    }

    public void SetPreviewTask(TaskViewModel taskToPreview)
    {
        PreviewedTask = taskToPreview;
        OnPropertyChanged(nameof(TaskTimeInformationText));
    }

    public void Stop()
    {
        if (State == TimerState.Idle)
        {
            return;
        }
        
        State = TimerState.Idle;
        var finishedSession = RunningSessionPart?.End();
        RunningSessionPart = null;
        if (finishedSession is not null && TaskToWorkOn is not null)
        {
            TaskToWorkOn.TaskModel.TimeSpent += finishedSession.Value.Duration;
            TaskToWorkOn = null;
            _refreshTimer.Stop();
            OnPropertyChanged(nameof(SessionTimeInformationText));
            OnPropertyChanged(nameof(TaskTimeInformationText));
        }
        
    }

    private void OnEveryRunningSecond(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(SessionTimeInformationText));
        OnPropertyChanged(nameof(TaskTimeInformationText));
    }

    public void Dispose()
    {
        _refreshTimer.Tick -= OnEveryRunningSecond;
        _refreshTimer.Stop();
    }
}
