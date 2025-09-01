using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Views.SessionViews;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class SessionViewModel: ViewModelBase
{
    public Session Session { get; }
    public TaskListViewModel Tasks { get; }
    public GroupListViewModel Groups { get; }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NextSubtaskOrTask))]
    private TaskViewModel? _selectedTask;

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
                return "Task: " + SelectedTask.TaskModel.Name;
            }
            else
            {
                return "Next subtask: " + SelectedTask.TaskModel.Subtasks.First(x => !x.IsCompleted).Name;
            }
        }
    }

    private readonly NotSelectedTaskSessionView _notSelectedView;
    
    private readonly SelectedTaskSessionView _selectedView;

    [ObservableProperty] private UserControl _currentSessionPage;

    public string TimeInformationText
    {
        get
        {
            return Session.TotalSessionTime().ToString(@"hh\:mm\:ss");
        }
    }

    public SessionViewModel(TaskListViewModel tasks, GroupListViewModel groups)
    {
        Session = new Session();
        Tasks = tasks;
        Groups = groups;
        
        _notSelectedView = new NotSelectedTaskSessionView()
        {
            DataContext = this
        };
        
        _selectedView = new SelectedTaskSessionView()
        {
            DataContext = this
        };

        _currentSessionPage = _notSelectedView;
    }
    
    [RelayCommand]
    private async Task SelectTaskAsync()
    {
        SelectedTask = await WeakReferenceMessenger.Default.Send(new SessionTaskSelectionRequest(Tasks));
        if (SelectedTask is not null)
        {
            CurrentSessionPage = _selectedView;
        }
        else
        {
            CurrentSessionPage = _notSelectedView;
        }
    }

    [RelayCommand]
    private void DeselectTask()
    {
        SelectedTask = null;
        CurrentSessionPage = _notSelectedView;
    }

    [RelayCommand]
    private void MarkNextDone()
    {
        if (SelectedTask!.TaskModel.Subtasks.All(x => x.IsCompleted))
        {
            SelectedTask.TaskModel.IsCompleted = true;
            // TODO show task done window
            DeselectTask();
        }
        else
        {
            SelectedTask.TaskModel.Subtasks.First(x => !x.IsCompleted).IsCompleted = true;
            OnPropertyChanged(nameof(NextSubtaskOrTask));
        }
    }

    [RelayCommand]
    private void EditTask()
    {
        WeakReferenceMessenger.Default.Send(new EditTaskInSessionMessage(SelectedTask!, Tasks));
        
    }

    public void EnsureSelectedTaskIsValid()
    {
        if (SelectedTask is not null && !SelectedTask!.TaskModel.Ready)
        {
            DeselectTask();
        }
    }
}
