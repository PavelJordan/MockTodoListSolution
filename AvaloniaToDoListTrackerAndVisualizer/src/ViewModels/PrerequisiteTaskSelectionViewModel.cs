using System.Collections.ObjectModel;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class PrerequisiteTaskSelectionViewModel: ViewModelBase
{
    public ObservableCollection<TaskModel> SelectedTasks { get; } 
    public TaskListViewModel AllTasks { get; }
    
    
    public PrerequisiteTaskSelectionViewModel(TaskViewModel task, TaskListViewModel allTasks)
    {
        SelectedTasks = new ObservableCollection<TaskModel>(task.TaskModel.Prerequisites.Collection);
        AllTasks = allTasks;
    }

    [RelayCommand]
    private void Close()
    {
        WeakReferenceMessenger.Default.Send(new ClosePrerequisiteSelectionMessage(SelectedTasks));
    }

    [RelayCommand]
    private void ToggleTaskSelection(TaskViewModel task)
    {
        // TODO ensure no cycles form
        if (SelectedTasks.Contains(task.TaskModel))
        {
            SelectedTasks.Remove(task.TaskModel);
        }
        else
        {
            SelectedTasks.Add(task.TaskModel);
        }
    }

    [RelayCommand]
    private void RemoveFromTaskSelection(TaskModel task)
    {
        SelectedTasks.Remove(task);
    }
}
