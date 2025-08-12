using System;
using System.ComponentModel;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class TaskViewModel: ViewModelBase, IDisposable
{
    public TaskModel TaskModel { get; }

    public TaskViewModel(TaskModel taskModel)
    {
        TaskModel = taskModel;
        TaskModel.PropertyChanged += ForwardPropertyChanged;
    }

    private void ForwardPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
            OnPropertyChanged(e.PropertyName);
    }

    public void Dispose()
    {
        TaskModel.PropertyChanged -= ForwardPropertyChanged;
    }
}
