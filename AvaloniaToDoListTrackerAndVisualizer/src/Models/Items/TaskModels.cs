using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using AvaloniaToDoListTrackerAndVisualizer.Wrappers;


namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

/// <summary>
/// Abstract class for both regular task (with group assigned, prerequisites and subtasks) and subtasks.
/// Everything is observable to reduce boilerplate by not repeating everything in ViewModel.
/// All property modification must happen on the UI thread!
/// </summary>
public abstract partial class TaskBaseModel: ObservableObject, ICompletable
{
    
    [ObservableProperty]
    private string _name;
    
    [ObservableProperty]
    private string? _description;
    
    /// <summary>
    /// Also affects Ready property, but that is handled in TaskModel as Ready is not reachable from here.
    /// </summary>
    [ObservableProperty]
    private bool _isCompleted = false;
    
    // TODO - validation for valid dates (begin <= deadlines, etc) (in specification)
    
    protected TaskBaseModel(string name)
    {
        _name = name;
    }
    
    
    public abstract bool CanChangeCompleteness { get; }
}

/// <summary>
/// The SubTask class. Is owned by TaskModel if created via AddSubtask. Only has name and description, and whether
/// it is marked completed. // TODO add more in the future with subtasks (not in specification)
/// </summary>
public sealed partial class SubTaskModel : TaskBaseModel
{
    /// <summary>
    /// Create new subtask for subtaskFor task. Returns the new subtask
    /// </summary>
    public SubTaskModel(string name) : base(name)
    { }
    

    
    public override bool CanChangeCompleteness { get; } = true;
}

/// <summary>
/// Represents main task - can be assigned to a group for color,
/// can have prerequisites as other top-level tasks and can also have preceding events that can block it.
/// For now, only this can have subtasks.
/// Can be marked ready only if prerequisites are completed and if it is after/on begin day.
/// Don't forget that TaskModel should be disposed.
/// </summary>
public sealed partial class TaskModel : TaskBaseModel, IDisposable, IHasId
{
    /// <summary>
    /// For persistent storage purposes
    /// </summary>
    public Guid Id { get; }
    
    [ObservableProperty]
    private Group? _group;
    
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
    
    public ReadOnlyObservableCollection<SubTaskModel> Subtasks { get; }
    
    private readonly ObservableChildrenCollectionWrapper<SubTaskModel> _subtasks = new();
    
    
    // TODO Enforce DAG!!! (no cycles, self-referencing...) (in specification)
    public ObservableChildrenCollectionWrapper<TaskModel> Prerequisites { get; } = new();
    
    public ObservableChildrenCollectionWrapper<PrecedingEvent> PrecedingEvents { get; } = new();
    
    public bool Ready => Prerequisites.Collection.All(x => x.IsCompleted) && PrecedingEvents.Collection.All(x => x.IsCompleted) && !IsCompleted && AfterBeginDate();
    
    public TaskModel(string name): this(name, Guid.NewGuid())
    { }

    public TaskModel(string name, Guid id): base(name)
    {
        Id = id;
        
        Prerequisites.Collection.CollectionChanged += OnCollectionChange;
        PrecedingEvents.Collection.CollectionChanged += OnCollectionChange;
        _subtasks.Collection.CollectionChanged += OnCollectionChange;
        Prerequisites.ChildrenPropertyChanged += OnChildChange;
        PrecedingEvents.ChildrenPropertyChanged += OnChildChange;
        _subtasks.ChildrenPropertyChanged += OnChildChange;

        PropertyChanged += CheckIfPropertiesShouldChange;
        
        Subtasks = new (_subtasks.Collection);
    }
    
    /// <summary>
    /// Adds newly created subtask. If the main task is completed, mark the subtask as completed too.
    /// Is called from the SubTaskModel constructor.
    /// </summary>
    public void AddSubtask(SubTaskModel subtask)
    {
        if (IsCompleted)
        {
            subtask.IsCompleted = true;
        }
        _subtasks.Collection.Add(subtask);
    }

    public void RemoveSubtask(SubTaskModel subtask)
    {
        _subtasks.Collection.Remove(subtask);
    }

    /// <summary>
    /// If "IsCompleted" property change, Ready and CanChangeCompleteness can change too. Notify subscribers
    /// </summary>
    private void CheckIfPropertiesShouldChange(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(IsCompleted))
        {
            // TODO - mark subtasks finished if IsCompleted is true
        }
        
        if (e.PropertyName is nameof(IsCompleted) or nameof(BeginDate))
        {
            OnPropertyChanged(nameof(Ready));
        }

        if (e.PropertyName is nameof(Ready))
        {
            OnPropertyChanged(nameof(CanChangeCompleteness));
        }
    }
    
    /// <summary>
    /// If prerequisites/preceding events are added or removed, ready property can change.
    /// Notify subscribers
    /// </summary>
    private void OnCollectionChange(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Ready));
    }

    /// <summary>
    /// If prerequisites/preceding events are newly completed or un-completed, ready property can change. Notify subscribers
    /// </summary>
    private void OnChildChange(object? sender, ChildrenPropertyChangedEventArgs e)
    {
        if (e.ChildrenEventArgs.PropertyName == nameof(ICompletable.IsCompleted)) OnPropertyChanged(nameof(Ready));
        // TODO Ensure we are not completed, if subtask changed to not completed (in specification)
    }

    /// <summary>
    /// Remove all subscriptions from the preceding events and prerequisites and dispose them too
    /// </summary>
    public void Dispose()
    {
        Prerequisites.Collection.CollectionChanged -= OnCollectionChange;
        PrecedingEvents.Collection.CollectionChanged -= OnCollectionChange;
        Prerequisites.ChildrenPropertyChanged -= OnChildChange;
        PrecedingEvents.ChildrenPropertyChanged -= OnChildChange;
        PropertyChanged -= CheckIfPropertiesShouldChange;
        Prerequisites.Dispose();
        PrecedingEvents.Dispose();
    }

    public override bool CanChangeCompleteness
    {
        get
        {
            return Ready || IsCompleted;
        }
    }

    private bool AfterBeginDate()
    {
        if (BeginDate is null)
        {
            return true;
        }
        else
        {
            return BeginDate.Value.Date <= DateTime.Now.Date;
        }
    }
    
}
