using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Views.SessionViews;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class SessionViewModel: ViewModelBase
{
    public TaskListViewModel Tasks { get; }
    public GroupListViewModel Groups { get; }
    
    [ObservableProperty]
    private TaskViewModel? _selectedTask;

    private readonly NotSelectedTaskSessionView _notSelectedView;
    
    private readonly SelectedTaskSessionView _selectedView;

    [ObservableProperty] private UserControl _currentSessionPage;

    public string TimeInformationText
    {
        get
        {
            return "Time information";
        }
    }

    public SessionViewModel(TaskListViewModel tasks, GroupListViewModel groups)
    {
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
        SelectedTask = await WeakReferenceMessenger.Default.Send(new SessionTaskSelectionRequest(Tasks, Groups));
        if (SelectedTask is not null)
        {
            CurrentSessionPage = _selectedView;
        }
        else
        {
            CurrentSessionPage = _notSelectedView;
        }
    }
}
