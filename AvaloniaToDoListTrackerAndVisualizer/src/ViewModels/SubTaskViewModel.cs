using System;
using System.ComponentModel;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

/// <summary>
/// Wrapper for sub-tasks which exposes its properties.
/// It also forwards property changed event for use in observable children collections,
/// because the collection keeps track of those events in its items.
/// All extra properties used for view will be here.
/// </summary>
public partial class SubTaskViewModel: ViewModelBase, IDisposable
{
    public SubTaskModel SubTask { get; }
    
    public SubTaskViewModel(SubTaskModel subTask)
    {
        SubTask = subTask;
        SubTask.PropertyChanged += ForwardPropertyChanged;
    }

    private void ForwardPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName);
    }

    public void Dispose()
    {
        SubTask.PropertyChanged -= ForwardPropertyChanged;
    }
}
