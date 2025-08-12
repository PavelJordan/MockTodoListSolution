using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using AvaloniaToDoListTrackerAndVisualizer.Decorators;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

public abstract partial class AbstractTaskModel(string name): ObservableObject, ICompletable
{
    [ObservableProperty]
    private string _name = name;
    
    [ObservableProperty]
    private string? _description;
    
    [ObservableProperty]
    private bool _isCompleted = false;
    
    [ObservableProperty]
    private DateTime? _beginDate;
    
    [ObservableProperty]
    private DateTime? _softDeadline;
    
    [ObservableProperty]
    private DateTime? _hardDeadline;
    
    [ObservableProperty]
    private TimeSpan _timeSpent = TimeSpan.Zero;
    
    [ObservableProperty]
    private TimeSpan? _timeExpected;
    
    private readonly ObservableCollection<SubTaskModel> _subtasks = new();
}

public sealed partial class TaskModel : AbstractTaskModel, IHasID, IDisposable
{
    public Guid ID { get; } = Guid.NewGuid();
    
    
    
    public RecursiveObservableCollectionDecorator<TaskModel> Prerequisites { get; } = new();

    public TaskModel? PrerequisiteFor
    {
        get
        {
            return null;
        }
    }

    public RecursiveObservableCollectionDecorator<PrecedingEvent> PrecedingEvents { get; } = new();
    
    public bool Ready => Prerequisites.Collection.All(x => x.IsCompleted) && PrecedingEvents.Collection.All(x => x.IsCompleted) && !IsCompleted;
    
    private readonly HashSet<ICompletable> _wiredItems = new();

    public TaskModel(string name): base(name)
    {
        Prerequisites.Collection.CollectionChanged += OnCollectionChange;
        PrecedingEvents.Collection.CollectionChanged += OnCollectionChange;
        Prerequisites.ChildrenPropertyChanged += OnChildChange;
        PrecedingEvents.ChildrenPropertyChanged += OnChildChange;

        PropertyChanged += CheckIfReadyShouldChange;
    }

    private void CheckIfReadyShouldChange(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsCompleted))
        {
            OnPropertyChanged(nameof(Ready));
        }
    }
    
    private void OnCollectionChange(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Ready));
    }

    private void OnChildChange(object? sender, object? child, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ICompletable.IsCompleted)) OnPropertyChanged(nameof(Ready));
    }

    public void Dispose()
    {
        Prerequisites.Collection.CollectionChanged -= OnCollectionChange;
        PrecedingEvents.Collection.CollectionChanged -= OnCollectionChange;
        Prerequisites.ChildrenPropertyChanged -= OnChildChange;
        PrecedingEvents.ChildrenPropertyChanged -= OnChildChange;
        PropertyChanged -= CheckIfReadyShouldChange;
    }
}

public sealed partial class SubTaskModel(string name, AbstractTaskModel subtaskFor) : AbstractTaskModel(name)
{
    [ObservableProperty]
    private AbstractTaskModel _subtaskFor = subtaskFor;
}
