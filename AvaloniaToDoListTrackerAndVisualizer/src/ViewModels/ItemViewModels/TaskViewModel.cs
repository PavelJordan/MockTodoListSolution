using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Media;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Wrappers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DynamicData;
using DynamicData.Binding;

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
    
    public GroupListViewModel Groups { get; }

    private readonly IDisposable _subTaskViewModelsPipeline;

    private readonly ReadOnlyObservableCollection<SubTaskViewModel> _subTasksViewModels;
    public ReadOnlyObservableCollection<SubTaskViewModel> SubTasksViewModels => _subTasksViewModels;

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

    public IBrush DeadlineInfoBackgroundColor
    {
        get
        {
            if (TaskModel.BeginDate is not null && TaskModel.BeginDate.Value.Date > DateTime.Now.Date)
            {
                return Brushes.LightGray;
            }
            
            if (TaskModel.SoftDeadline is not null && TaskModel.SoftDeadline.Value.Date >= DateTime.Now.Date)
            {
                return Brushes.LightGreen;
            }
            
            if (TaskModel.HardDeadline is not null && TaskModel.HardDeadline.Value.Date >= DateTime.Now.Date)
            {
                return Brushes.Orange;
            }

            if (TaskModel.HardDeadline is not null)
            {
                return Brushes.Red;
            }
            
            if (TaskModel.SoftDeadline is not null)
            {
                return Brushes.Yellow;
            }
            
            if (TaskModel.BeginDate is not null)
            {
                return Brushes.YellowGreen;
            }

            return Brushes.Gray;
        }
    }

    public string DeadlineInfoText
    {
        get
        {
            if (TaskModel.BeginDate is not null && TaskModel.BeginDate.Value.Date > DateTime.Now.Date)
            {
                return (TaskModel.BeginDate.Value.Date - DateTime.Now.Date).Days + " " + Localization.DaysUntilBeginDate;
            }
            
            if (TaskModel.SoftDeadline is not null && TaskModel.SoftDeadline.Value.Date >= DateTime.Now.Date)
            {
                return (TaskModel.SoftDeadline.Value.Date - DateTime.Now.Date).Days + " " + Localization.DaysUntilSoftDeadline;
            }
            
            if (TaskModel.HardDeadline is not null && TaskModel.HardDeadline.Value.Date >= DateTime.Now.Date)
            {
                return (TaskModel.HardDeadline.Value.Date - DateTime.Now.Date).Days + " " + Localization.DaysUntilHardDeadline;
            }

            if (TaskModel.HardDeadline is not null)
            {
                return (DateTime.Now.Date - TaskModel.HardDeadline.Value.Date).Days + " " + Localization.DaysAfterHard;
            }
            
            if (TaskModel.SoftDeadline is not null)
            {
                return (DateTime.Now.Date - TaskModel.SoftDeadline.Value.Date).Days + " " + Localization.DaysAfterSoft;
            }

            if (TaskModel.BeginDate is not null)
            {
                return (DateTime.Now.Date - TaskModel.BeginDate.Value.Date).Days + " " + Localization.DaysAfterBeginDateNoDeadline;
            }

            return Localization.NoDeadlineSet;
        }
    }

    public string TimeLeftText
    {
        get
        {
            return formatTimeSpan(TaskModel.TimeSpent) + "/" + formatTimeSpan(TaskModel.TimeExpected);
        }
    }

    private string formatTimeSpan(TimeSpan? timeSpan)
    {
        if (timeSpan is null)
        {
            return Localization.NotSetText;
        }
        else
        {
            return timeSpan.Value.ToString(@"hh\:mm");
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

    public string GroupText
    {
        get { return TaskModel.Group is null ? Localization.SelectGroupText : TaskModel.Group.GroupName; }
    }
    
    public ISolidColorBrush GroupColor
    {
        get { return TaskModel.Group is null ? Brushes.Gray : new SolidColorBrush(TaskModel.Group.GroupColor); }
    }
    
    public bool CanChangeCompleteness => TaskModel.CanChangeCompleteness;
    

    [RelayCommand(CanExecute = nameof(CanChangeCompleteness))]
    private void CompleteButtonClicked()
    {
        TaskModel.IsCompleted = !TaskModel.IsCompleted;
    }

    public TaskViewModel(TaskModel taskModel, GroupListViewModel groups, LocalizationProvider localization)
    {
        TaskModel = taskModel;
        Groups = groups;
        Localization = localization;
        TaskModel.PropertyChanged += UpdateViewModelProperties;
        // For ObservableChildrenCollection
        TaskModel.PropertyChanged += ForwardPropertyChangedEvent;
        Localization.PropertyChanged += UpdateLocal;

        _subTaskViewModelsPipeline = taskModel.Subtasks
            .ToObservableChangeSet()
            .Transform(subTaskModel => new SubTaskViewModel(subTaskModel, Localization))
            .Bind(out _subTasksViewModels)
            .DisposeMany()
            .Subscribe();

        Groups.AllGroups.Collection.CollectionChanged += DeleteGroupAssignmentIfGroupDeleted;
        Groups.AllGroups.ChildrenPropertyChanged += UpdateGroupProperties;

    }

    private void ForwardPropertyChangedEvent(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName);
    }

    
    private void UpdateLocal(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(CompleteButtonText));
        OnPropertyChanged(nameof(ActionButtonText));
        OnPropertyChanged(nameof(DeadlineInfoText));
        OnPropertyChanged(nameof(GroupText));
        OnPropertyChanged(nameof(TimeLeftText));
    }

    /// <summary>
    /// Notify appropriate subscribers of view model properties if the wrapped task model changed.
    /// </summary>
    private void UpdateViewModelProperties(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(TaskModel.IsCompleted))
        {
            OnPropertyChanged(nameof(CompleteButtonText));
            OnPropertyChanged(nameof(CompleteButtonColor));
        }

        if (e.PropertyName is nameof(TaskModel.CanChangeCompleteness))
        {
            CompleteButtonClickedCommand.NotifyCanExecuteChanged();
        }

        if (e.PropertyName is nameof(TaskModel.SoftDeadline) or nameof(TaskModel.HardDeadline) or nameof(TaskModel.BeginDate))
        {
            OnPropertyChanged(nameof(DeadlineInfoBackgroundColor));
            OnPropertyChanged(nameof(DeadlineInfoText));
        }

        if (e.PropertyName is nameof(TaskModel.TimeExpected))
        {
            OnPropertyChanged(nameof(TimeLeftText));
        }
        
        if (e.PropertyName is nameof(TaskModel.Group))
        {
            OnPropertyChanged(nameof(GroupText));
            OnPropertyChanged(nameof(GroupColor));
        }
    }

    private void DeleteGroupAssignmentIfGroupDeleted(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (TaskModel.Group is Group group && !Groups.AllGroups.Collection.Contains(group))
        {
            TaskModel.Group = null;
        }
    }
    
    private void UpdateGroupProperties(object? sender, ChildrenPropertyChangedEventArgs e)
    {
        if (e.Child == TaskModel.Group)
        {
            OnPropertyChanged(nameof(GroupText));
            OnPropertyChanged(nameof(GroupColor));
        }
    }
    

    public void Dispose()
    {
        TaskModel.PropertyChanged -= UpdateViewModelProperties;
        TaskModel.PropertyChanged -= ForwardPropertyChangedEvent;
        Localization.PropertyChanged -= UpdateLocal;
        _subTaskViewModelsPipeline.Dispose();
    }

    public List<TaskViewModel> FindCyclingNeighbors()
    {
        // TODO implement
        return [];
    }
}
