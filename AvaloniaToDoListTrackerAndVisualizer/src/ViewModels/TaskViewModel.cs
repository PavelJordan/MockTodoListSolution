using System;
using System.ComponentModel;
using Avalonia.Media;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

/// <summary>
/// Wrapper for tasks which exposes its properties.
/// It also forwards property changed event for use in observable children collections,
/// because the collection keeps track of those events in its items.
/// All extra properties used for view will be here.
/// </summary>
public partial class TaskViewModel: ViewModelBase, IDisposable
{
    public TaskModel TaskModel { get; }

    public IBrush CompleteButtonColor
    {
        get
        {
            if (!TaskModel.IsCompleted)
            {
                return Brushes.LimeGreen;
            }
            else
            {
                return Brushes.Red;
            }
        }
    }
    
    public string CompleteButtonText
    {
        get
        {
            if (!TaskModel.IsCompleted)
            {
                return "Mark done";
            }
            else
            {
                return "Mark not done";
            }
        }
    }
    
    public bool CanChangeCompleteness => TaskModel.CanChangeCompleteness;
    

    [RelayCommand(CanExecute = nameof(CanChangeCompleteness))]
    private void CompleteButtonClicked()
    {
        TaskModel.IsCompleted = !TaskModel.IsCompleted;
    }

    public TaskViewModel(TaskModel taskModel)
    {
        TaskModel = taskModel;
        TaskModel.PropertyChanged += ForwardPropertyChanged;
        TaskModel.PropertyChanged += UpdateViewModelProperties;
    }

    private void ForwardPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
            OnPropertyChanged(e.PropertyName);
    }

    /// <summary>
    /// Notify appropriate subscribers of view model properties if the wrapped task model changed.
    /// </summary>
    private void UpdateViewModelProperties(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TaskModel.IsCompleted))
        {
            OnPropertyChanged(nameof(CompleteButtonText));
            OnPropertyChanged(nameof(CompleteButtonColor));
        }

        if (e.PropertyName == nameof(TaskModel.CanChangeCompleteness))
        {
            CompleteButtonClickedCommand.NotifyCanExecuteChanged();
        }
    }
    

    public void Dispose()
    {
        TaskModel.PropertyChanged -= ForwardPropertyChanged;
    }
}
