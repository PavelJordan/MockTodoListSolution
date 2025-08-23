using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Media;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;



/// <summary>
/// Wrapper for tasks which exposes its properties.
/// It also forwards property changed event for use in observable children collections,
/// because the collection keeps track of those events in its items.
/// All extra properties used for view will be here.
/// </summary>
public partial class TaskViewModel: ViewModelBase, IDisposable
{
    
    public enum ActionButtonMode {
        Details, Delete
    }
    
    public TaskModel TaskModel { get; }

    public LocalizationProvider Localization { get; }

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
                return Localization.CompleteButton;
            }
            else
            {
                return Localization.UnCompleteButton;
            }
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ActionButtonColor))]
    [NotifyPropertyChangedFor(nameof(ActionButtonText))]
    private ActionButtonMode _actionMode = ActionButtonMode.Details;

    public IBrush ActionButtonColor
    {
        get
        {
            switch (ActionMode)
            {
                case ActionButtonMode.Details: return Brushes.DimGray;
                case ActionButtonMode.Delete: return Brushes.Red;
                default: return Brushes.Gray;
            }
        }
    }

    public string ActionButtonText
    {
        get
        {
            switch (ActionMode)
            {
                case ActionButtonMode.Details: return Localization.DetailsButton;
                case ActionButtonMode.Delete: return Localization.DeleteButton;
                default: return "Action";
            }
        }
    }
    
    [RelayCommand]
    private async Task ActionButtonPress()
    {
        switch (ActionMode)
        {
            case ActionButtonMode.Details:
                await WeakReferenceMessenger.Default.Send(new EditTaskMessage(this, false));
                break;
            case ActionButtonMode.Delete:
                WeakReferenceMessenger.Default.Send(new DeleteTaskViewModelRequest(this));
                break;
        }
    }
    
    public bool CanChangeCompleteness => TaskModel.CanChangeCompleteness;
    

    [RelayCommand(CanExecute = nameof(CanChangeCompleteness))]
    private void CompleteButtonClicked()
    {
        TaskModel.IsCompleted = !TaskModel.IsCompleted;
    }

    public TaskViewModel(TaskModel taskModel, LocalizationProvider localization)
    {
        TaskModel = taskModel;
        Localization = localization;
        TaskModel.PropertyChanged += ForwardPropertyChanged;
        TaskModel.PropertyChanged += UpdateViewModelProperties;
        Localization.PropertyChanged += UpdateLocal;
    }

    private void ForwardPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
            OnPropertyChanged(e.PropertyName);
    }
    
    private void UpdateLocal(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(CompleteButtonText));
        OnPropertyChanged(nameof(ActionButtonText));
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
