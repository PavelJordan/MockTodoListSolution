using System;
using System.ComponentModel;
using Avalonia.Media;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

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
    public LocalizationProvider Localization { get; }
    
    public SubTaskViewModel(SubTaskModel subTask, LocalizationProvider localization)
    {
        Localization = localization;
        SubTask = subTask;
        SubTask.PropertyChanged += UpdateViewModelProperties;
        // For ObservableChildrenCollection
        SubTask.PropertyChanged += ForwardPropertyChangedEvent;
    }

    public void Dispose()
    {
        SubTask.PropertyChanged -= UpdateViewModelProperties;
        SubTask.PropertyChanged -= ForwardPropertyChangedEvent;
    }

    public string CompleteButtonText
    {
        get
        {
            if (SubTask.IsCompleted)
            {
                return Localization.UnCompleteButton;
            }
            else
            {
                return Localization.CompleteButton;
            }
        }
    }
    
    public IBrush CompleteButtonBackground
    {
        get
        {
            if (SubTask.IsCompleted)
            {
                return Brushes.Red;
            }
            else
            {
                return Brushes.LimeGreen;
            }
        }
    }

    /// <summary>
    /// For ObservableChildrenCollection
    /// </summary>
    private void ForwardPropertyChangedEvent(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName);
    }

    /// <summary>
    /// Update text and background if IsCompleted was changed in subtask
    /// </summary>
    private void UpdateViewModelProperties(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SubTask.IsCompleted))
        {
            OnPropertyChanged(nameof(CompleteButtonText));
            OnPropertyChanged(nameof(CompleteButtonBackground));
        }
    }

    [RelayCommand]
    private void DoneButtonClicked()
    {
        SubTask.IsCompleted = !SubTask.IsCompleted;
    }
    
    [RelayCommand]
    private void EditSubtask()
    {
        WeakReferenceMessenger.Default.Send(new EditSubTaskMessage(this));
    }
}
