using AvaloniaToDoListTrackerAndVisualizer.Messages;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class SessionTaskSelectionViewModel: ViewModelBase
{
    public TaskListViewModel AllTasks { get; }
    
    public SessionTaskSelectionViewModel(TaskListViewModel allTasks)
    {
        AllTasks = allTasks;
    }

    [RelayCommand]
    private void SelectTask(TaskViewModel task)
    {
        WeakReferenceMessenger.Default.Send(new CloseSessionTaskSelectionMessage(task));
    }

    [RelayCommand]
    private void Cancel()
    {
        WeakReferenceMessenger.Default.Send(new CloseSessionTaskSelectionMessage(null));
    }
}
