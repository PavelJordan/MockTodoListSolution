using System;
using Avalonia.Threading;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public enum TimerState
{
    Idle, Work, Break
}

public enum TimerType
{
    RegularTimer, PomodoroTimer
}

public partial class TimerViewModel: ViewModelBase, IDisposable
{

    private static bool FalseConstant { get; } = false;
    
    public Session CurrentSession { get; } = new Session();
    
    public TimerState State { get; private set; } = TimerState.Idle;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedTimer))]
    private bool _regularTimerSelected = true;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedTimer))]
    private bool _pomodoroTimerSelected = false;

    private readonly RegularTimerViewModel _regularTimerViewModel;
    private readonly PomodoroTimerViewModel _pomodoroTimerViewModel;

    public ViewModelBase SelectedTimer
    {
        get
        {
            if (PomodoroTimerSelected)
            {

                return _pomodoroTimerViewModel;
            }
            return _regularTimerViewModel;
        }
    }
    
    public Session.RunningSessionPart? RunningSessionPart { get; private set; }
    
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
                   (RunningSessionPart?.TimeSoFar ?? TimeSpan.Zero);
        }
    }

    public string SessionTimeInformationText
    {
        get
        {
            return CurrentSession.TotalSessionTime().ToString(@"hh\:mm\:ss");
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
        _pomodoroTimerViewModel = new(this);
        _regularTimerViewModel = new(this);
        
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
    
    public void RefreshTimerProperties()
    {
        OnPropertyChanged(nameof(TaskTimeInformationText));
        OnPropertyChanged(nameof(SelectedTimer));
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
