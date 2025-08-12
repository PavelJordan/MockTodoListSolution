using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using AvaloniaToDoListTrackerAndVisualizer.Decorators;
using AvaloniaToDoListTrackerAndVisualizer.Extensions;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class TaskListViewModel: ViewModelBase, IDisposable
{
    public RecursiveObservableCollectionDecorator<TaskViewModel> AllTasks { get; } = new();
    
    public ReadOnlyObservableCollection<TaskViewModel> ReadyTasks { get; }
    public ReadOnlyObservableCollection<TaskViewModel> CompletedTasks { get; }
    private readonly ObservableCollection<TaskViewModel> _readyTasks = new();
    private readonly ObservableCollection<TaskViewModel> _completedTasks = new();
    
    public TaskListViewModel()
    {
        ReadyTasks = new (_readyTasks);
        CompletedTasks = new (_completedTasks);

        AllTasks.Collection.CollectionChanged += OnAllTasksManipulation;
        AllTasks.ChildrenPropertyChanged += OnTaskManipulation;
    }
    

    private void OnTaskManipulation(object? senderCollection, object? task, PropertyChangedEventArgs e)
    {
        if (task is TaskViewModel taskViewModel)
        {
            if (e.PropertyName == nameof(TaskModel.IsCompleted))
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
            
            if (e.PropertyName == nameof(TaskModel.Ready))
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
            throw new ApplicationException("Bad task manipulation");
        }
    }

    private void OnAllTasksManipulation(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Replace)
        {
            UnregisterTasks(e.OldItems?.OfType<TaskViewModel>() ?? Enumerable.Empty<TaskViewModel>());
        }
            
        if (e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Replace)
        {
            RegisterTasks(e.NewItems?.OfType<TaskViewModel>() ?? Enumerable.Empty<TaskViewModel>());
        }

        if (e.Action is NotifyCollectionChangedAction.Reset)
        {
            _readyTasks.Clear();
            _completedTasks.Clear();
            RegisterTasks(AllTasks.Collection);
        }
    }

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

    private void UnregisterTasks(IEnumerable<TaskViewModel> tasks)
    {
        foreach (var item in tasks)
        {
            _readyTasks.Remove(item);
            _completedTasks.Remove(item);
        }
    }

    public void Dispose()
    {
        AllTasks.Collection.CollectionChanged -= OnAllTasksManipulation;
        AllTasks.ChildrenPropertyChanged -= OnTaskManipulation;
    }
}

