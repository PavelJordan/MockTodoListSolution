using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using AvaloniaToDoListTrackerAndVisualizer.Wrappers;
using AvaloniaToDoListTrackerAndVisualizer.Extensions;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

/// <summary>
/// ViewModel with list of tasks and readonly list of ready tasks and completed tasks, which act as filtered
/// collections. It uses ObservableChildrenCollectionWrapper to update the filtered collections. The items
/// are actually ViewModels of tasks.
/// </summary>
public class TaskListViewModel: ViewModelBase, IDisposable
{
    /// <summary>
    /// All registered tasks
    /// </summary>
    public ObservableChildrenCollectionWrapper<TaskViewModel> AllTasks { get; } = new();
    
    /// <summary>
    /// Readonly view of collection inside AllTasks wrapper
    /// </summary>
    public ReadOnlyObservableCollection<TaskViewModel> AllTasksReadOnly { get; }
    
    /// <summary>
    /// Registered tasks which fall into "ready" category - ready and not completed
    /// </summary>
    public ReadOnlyObservableCollection<TaskViewModel> ReadyTasks { get; }
    
    /// <summary>
    /// Registered tasks which fall into "completed" category - completed
    /// </summary>
    public ReadOnlyObservableCollection<TaskViewModel> CompletedTasks { get; }
    
    
    private readonly ObservableCollection<TaskViewModel> _readyTasks = new();
    
    private readonly ObservableCollection<TaskViewModel> _completedTasks = new();
    
    public TaskListViewModel()
    {
        ReadyTasks = new (_readyTasks);
        CompletedTasks = new (_completedTasks);
        AllTasksReadOnly = new (AllTasks.Collection);

        AllTasks.Collection.CollectionChanged += OnAllTasksManipulation;
        AllTasks.ChildrenPropertyChanged += OnTaskManipulation;
    }
    
    /// <summary>
    /// Action to do when a task inside AllTasks changes (registered task). Reconsider whenever it still is
    /// ready or completed.
    /// </summary>
    private void OnTaskManipulation(object? sender, ChildrenPropertyChangedEventArgs e)
    {
        if (e.Child is TaskViewModel taskViewModel)
        {
            if (e.ChildrenEventArgs.PropertyName == nameof(TaskModel.IsCompleted))
            {
                if (taskViewModel.TaskModel.IsCompleted)
                {
                    _completedTasks.AddIfNotExists(taskViewModel);
                }
                else
                {
                    _completedTasks.Remove(taskViewModel);
                }
            }
            
            if (e.ChildrenEventArgs.PropertyName == nameof(TaskModel.Ready))
            {
                if (taskViewModel.TaskModel.Ready)
                {
                    _readyTasks.AddIfNotExists(taskViewModel);
                }
                else
                {
                    _readyTasks.Remove(taskViewModel);
                }
            }
        }
        else
        {
            throw new ApplicationException("Bad task manipulation"); // TODO make as assertion/error log
        }
    }

    /// <summary>
    /// Action to do when the collection of all tasks is changed (item added, removed, collection reset, etc...)
    /// </summary>
    private void OnAllTasksManipulation(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Unregister removed tasks
        if (e.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Replace)
        {
            UnregisterTasks(e.OldItems?.OfType<TaskViewModel>() ?? Enumerable.Empty<TaskViewModel>());
        }
            
        // Register new tasks
        if (e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Replace)
        {
            RegisterTasks(e.NewItems?.OfType<TaskViewModel>() ?? Enumerable.Empty<TaskViewModel>());
        }

        // If reset, clear registrations and reregister everything
        if (e.Action is NotifyCollectionChangedAction.Reset)
        {
            _readyTasks.Clear();
            _completedTasks.Clear();
            RegisterTasks(AllTasks.Collection);
        }
    }

    /// <summary>
    /// Register task - consider its readiness and whether its completed
    /// </summary>
    private void RegisterTasks(IEnumerable<TaskViewModel> tasks)
    {
        foreach (var item in tasks)
        {
            if (item.TaskModel.Ready)
            {
                _readyTasks.AddIfNotExists(item);
            }

            if (item.TaskModel.IsCompleted)
            {
                _completedTasks.AddIfNotExists(item);
            }
        }
    }

    /// <summary>
    /// Unregister task - remove from both ready and completed tasks (if they are there)
    /// </summary>
    private void UnregisterTasks(IEnumerable<TaskViewModel> tasks)
    {
        foreach (var item in tasks)
        {
            _readyTasks.Remove(item);
            _completedTasks.Remove(item);
        }
    }

    // TODO: Dispose when window closes
    public void Dispose()
    {
        foreach (var item in AllTasksReadOnly)
        {
            item.Dispose();
        }
        AllTasks.Collection.CollectionChanged -= OnAllTasksManipulation;
        AllTasks.ChildrenPropertyChanged -= OnTaskManipulation;
        AllTasks.Dispose();
    }
}

