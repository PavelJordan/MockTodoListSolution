using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

/// <summary>
/// SessionViewModel is used to display timer (regular or pomodoro) together with task information
/// and buttons for start/stop, settings, home, etc.
/// </summary>
public partial class SessionViewModel: ViewModelBase
{
    public TaskListViewModel Tasks { get; }
    public GroupListViewModel Groups { get; }
    public TimerViewModel Timer { get; }
    
    private const string StopButtonPath = "M17.75,7 C19.5449254,7 21,8.45507456 21,10.25 L21,37.75 C21,39.5449254 19.5449254,41 17.75,41 L12.25,41 C10.4550746,41 9,39.5449254 9,37.75 L9,10.25 C9,8.45507456 10.4550746,7 12.25,7 L17.75,7 Z M35.75,7 C37.5449254,7 39,8.45507456 39,10.25 L39,37.75 C39,39.5449254 37.5449254,41 35.75,41 L30.25,41 C28.4550746,41 27,39.5449254 27,37.75 L27,10.25 C27,8.45507456 28.4550746,7 30.25,7 L35.75,7 Z M17.75,9.5 L12.25,9.5 C11.8357864,9.5 11.5,9.83578644 11.5,10.25 L11.5,37.75 C11.5,38.1642136 11.8357864,38.5 12.25,38.5 L17.75,38.5 C18.1642136,38.5 18.5,38.1642136 18.5,37.75 L18.5,10.25 C18.5,9.83578644 18.1642136,9.5 17.75,9.5 Z M35.75,9.5 L30.25,9.5 C29.8357864,9.5 29.5,9.83578644 29.5,10.25 L29.5,37.75 C29.5,38.1642136 29.8357864,38.5 30.25,38.5 L35.75,38.5 C36.1642136,38.5 36.5,38.1642136 36.5,37.75 L36.5,10.25 C36.5,9.83578644 36.1642136,9.5 35.75,9.5 Z";
    private const string PlayButtonPath = "M13.7501344,8.41212026 L38.1671892,21.1169293 C39.7594652,21.9454306 40.3786269,23.9078584 39.5501255,25.5001344 C39.2420737,26.0921715 38.7592263,26.5750189 38.1671892,26.8830707 L13.7501344,39.5878797 C12.1578584,40.4163811 10.1954306,39.7972194 9.36692926,38.2049434 C9.12586301,37.7416442 9,37.2270724 9,36.704809 L9,11.295191 C9,9.50026556 10.4550746,8.045191 12.25,8.045191 C12.6976544,8.045191 13.1396577,8.13766178 13.5485655,8.31589049 L13.7501344,8.41212026 Z M12.5961849,10.629867 L12.4856981,10.5831892 C12.4099075,10.5581 12.3303482,10.545191 12.25,10.545191 C11.8357864,10.545191 11.5,10.8809774 11.5,11.295191 L11.5,36.704809 C11.5,36.8253313 11.5290453,36.9440787 11.584676,37.0509939 C11.7758686,37.4184422 12.2287365,37.5613256 12.5961849,37.370133 L37.0132397,24.665324 C37.1498636,24.5942351 37.2612899,24.4828088 37.3323788,24.3461849 C37.5235714,23.9787365 37.380688,23.5258686 37.0132397,23.334676 L12.5961849,10.629867 Z";

    /// <summary>
    /// Task selected to be worked on (first only previewed in timer)
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NextSubtaskOrTask))]
    private TaskViewModel? _selectedTask;

    /// <summary>
    /// If there are some unfinished subtasks in the selected task, show it. Otherwise, show the task itself.
    /// No task selected variant should not happen - button for selection should be displayed instead
    /// </summary>
    public string NextSubtaskOrTask
    {
        get
        {
            if (SelectedTask is null)
            {
                return "No task selected";
            }
            else if (SelectedTask.TaskModel.Subtasks.All(x => x.IsCompleted))
            {
                return Tasks.Localization.TaskText + ": " + SelectedTask.TaskModel.Name;
            }
            else
            {
                // Get first uncompleted subtask
                return Tasks.Localization.SubtaskText +": " + SelectedTask.TaskModel.Subtasks.First(x => !x.IsCompleted).Name;
            }
        }
    }

    private readonly NotSelectedTaskSessionViewModel _notSelectedViewModel;
    
    private readonly SelectedTaskSessionViewModel _selectedViewModel;

    /// <summary>
    /// Avalonia will find correct view for you, if you bind here :) (ViewLocator)
    /// </summary>
    [ObservableProperty] private ViewModelBase _currentSessionPage;
    

    public SessionViewModel(TaskListViewModel tasks, GroupListViewModel groups, Session session)
    {
        Tasks = tasks;
        Groups = groups;
        Timer = new TimerViewModel(session, Tasks.Localization);

        _notSelectedViewModel = new(this);

        _selectedViewModel = new(this);

        _currentSessionPage = _notSelectedViewModel;
    }
    
    /// <summary>
    /// Show dialog window to select task. If not successful, still show the Select task button.
    /// </summary>
    [RelayCommand]
    private async Task SelectTaskAsync()
    {
        SelectedTask = await WeakReferenceMessenger.Default.Send(new SessionTaskSelectionRequest(Tasks));
        if (SelectedTask is not null)
        {
            Timer.SetPreviewTask(SelectedTask);
            CurrentSessionPage = _selectedViewModel;
        }
        else
        {
            CurrentSessionPage = _notSelectedViewModel;
        }
    }

    [RelayCommand]
    private void DeselectTask()
    {
        StopTimer();
        SelectedTask = null;
        CurrentSessionPage = _notSelectedViewModel;
    }

    /// <summary>
    /// Mark next subtask done, if there is one unfinished. Otherwise,
    /// mark the task done and deselect it.
    /// </summary>
    [RelayCommand]
    private void MarkNextDone()
    {
        if (SelectedTask!.TaskModel.Subtasks.All(x => x.IsCompleted))
        {
            // Complete the task and unselect it
            SelectedTask.TaskModel.IsCompleted = true;
            DeselectTask();
        }
        else
        {
            // Complete next subtask and move on
            SelectedTask.TaskModel.Subtasks.First(x => !x.IsCompleted).IsCompleted = true;
            OnPropertyChanged(nameof(NextSubtaskOrTask));
        }
    }

    [RelayCommand]
    private void EditTask()
    {
        StopTimer();
        WeakReferenceMessenger.Default.Send(new EditTaskInSessionMessage(SelectedTask!, Tasks));
    }

    /// <summary>
    /// Ensure that selected task is not null and that it is ready
    /// </summary>
    public void EnsureSelectedTaskIsValid()
    {
        if (SelectedTask is not null && !SelectedTask!.TaskModel.Ready)
        {
            DeselectTask();
        }
        else
        {
            OnPropertyChanged(nameof(NextSubtaskOrTask));
            Timer.RefreshTimerProperties();
        }
    }

    /// <summary>
    /// Start/Stop toggle. To know whether running, check Timer.Idle property.
    /// </summary>
    [RelayCommand]
    private void StartOrStopTimer()
    {
        if (Timer.Idle)
        {
            StartTimer();
        }
        else
        {
            StopTimer();
        }
    }

    private void StartTimer()
    {
        Timer.Start(SelectedTask!);
        OnPropertyChanged(nameof(PlayOrStopStreamGeometry));
    }

    private void StopTimer()
    {
        Timer.Stop();
        OnPropertyChanged(nameof(PlayOrStopStreamGeometry));
    } 
    
    
    public StreamGeometry PlayOrStopStreamGeometry
    {
        get
        {
            if (Timer.Idle)
            {
                return StreamGeometry.Parse(PlayButtonPath);
            }
            else
            {
                return StreamGeometry.Parse(StopButtonPath);
            }
        }
    }

    /// <summary>
    /// Show dialog window where user can set the timer (regular or pomodoro? How long?) etc.
    /// </summary>
    [RelayCommand]
    private void SetupTimer()
    {
        StopTimer();
        WeakReferenceMessenger.Default.Send(new SetupTimerRequest(Timer));
    }
}
