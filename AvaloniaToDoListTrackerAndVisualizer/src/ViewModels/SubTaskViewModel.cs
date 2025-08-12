using System.ComponentModel;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class SubTaskViewModel: ViewModelBase
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
}
