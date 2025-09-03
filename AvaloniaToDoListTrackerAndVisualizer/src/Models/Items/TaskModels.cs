using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using AvaloniaToDoListTrackerAndVisualizer.Wrappers;


namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

/// <summary>
/// Abstract class for both regular task (with group assigned, prerequisites and subtasks) and subtasks.
/// Everything is observable to reduce boilerplate by not repeating everything in ViewModel.
/// All property modification must happen on the UI thread!
/// </summary>
public abstract partial class TaskBaseModel: ObservableValidator, ICompletable
{
    
    [ObservableProperty]
    [Required]
    [NotifyDataErrorInfo]
    private string _name;
    
    [ObservableProperty]
    private string? _description;
    
    /// <summary>
    /// Also affects Ready property, but that is handled in TaskModel as Ready is not reachable from here.
    /// </summary>
    [ObservableProperty]
    private bool _isCompleted = false;
    
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
public sealed class SubTaskModel : TaskBaseModel
{
    /// <summary>
    /// Create new subtask for subtaskFor task. Returns the new subtask
    /// </summary>
    public SubTaskModel(string name) : base(name)
    { }
    
    /// <summary>
    /// Subtask can be always marked completed/not completed.
    /// TODO in the future, only if parent task is not completed
    /// </summary>
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
    
    // TODO - validation for valid dates (begin <= deadlines, etc)
    
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
    
    /// <summary>
    /// Only read-only is exposed to force AddSubtask method usage
    /// </summary>
    public ReadOnlyObservableCollection<SubTaskModel> Subtasks { get; }
    
    private readonly ObservableChildrenCollectionWrapper<SubTaskModel> _subtasks = new();
    
    
    // TODO Enforce DAG (no cycles, self-referencing...) (harmless, user just blocks the tasks in cycle and can unblock)
    public ObservableChildrenCollectionWrapper<TaskModel> Prerequisites { get; } = new();
    
    // TODO implement
    public ObservableChildrenCollectionWrapper<PrecedingEvent> PrecedingEvents { get; } = new();
    
    /// <summary>
    /// Task is ready if all prerequisites and events are completed, and if it is after (or on) begin date
    /// </summary>
    public bool Ready => Prerequisites.Collection.All(x => x.IsCompleted) && PrecedingEvents.Collection.All(x => x.IsCompleted) && !IsCompleted && AfterBeginDate();
    
    /// <summary>
    /// Create new task with the specified name (and with new id)
    /// </summary>
    public TaskModel(string name): this(name, Guid.NewGuid())
    { }

    /// <summary>
    /// Reconstruct task with the specified name and id.
    /// </summary>
    public TaskModel(string name, Guid id): base(name)
    {
        Id = id;
        
        RegisterToEvents();

        // Only readonly is exposed to enforce AddSubtask method usage
        Subtasks = new (_subtasks.Collection);
    }

    /// <summary>
    /// We need to listen in case prerequisites or events change IsCompleted state (for our ready)
    /// Or if new one is added (or removed, etc.)
    /// </summary>
    private void RegisterToEvents()
    {
        
        Prerequisites.Collection.CollectionChanged += OnCollectionChange;
        PrecedingEvents.Collection.CollectionChanged += OnCollectionChange;
        Prerequisites.ChildrenPropertyChanged += OnChildChange;
        PrecedingEvents.ChildrenPropertyChanged += OnChildChange;

        PropertyChanged += CheckIfPropertiesShouldChange;
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

    /// <summary>
    /// Simply try to remove the subtask
    /// </summary>
    public void RemoveSubtask(SubTaskModel subtask)
    {
        _subtasks.Collection.Remove(subtask);
    }

    /// <summary>
    /// If "IsCompleted" property change on Prerequisites or events,
    /// Ready and CanChangeCompleteness can change too. Notify subscribers.
    /// On the other hand, change of Ready implies possible change
    /// of CanChangeCompleteness.
    /// </summary>
    private void CheckIfPropertiesShouldChange(object? sender, PropertyChangedEventArgs e)
    {
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
        if (e.ChildrenEventArgs.PropertyName == nameof(ICompletable.IsCompleted))
        {
            OnPropertyChanged(nameof(Ready));
        }
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
        _subtasks.Dispose();
    }

    /// <summary>
    /// Completeness can be changed if Ready or IsCompleted changes
    /// </summary>
    public override bool CanChangeCompleteness
    {
        get
        {
            return Ready || IsCompleted;
        }
    }

    /// <summary>
    /// If begin date is null, it is always true. Otherwise,
    /// true only if begin date is at least today
    /// </summary>
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

    /// <summary>
    /// Move subtask up inside the subtask list (only if possible)
    /// </summary>
    public void MoveSubtaskUp(SubTaskModel subtask)
    {
        int subtaskIndex = _subtasks.Collection.IndexOf(subtask);
        if (subtaskIndex > 0)
        {
            _subtasks.Collection.Move(subtaskIndex, subtaskIndex - 1);
        }
    }
    
    /// <summary>
    /// Move subtask down inside the subtask list (only if possible)
    /// </summary>
    public void MoveSubtaskDown(SubTaskModel subtask)
    {
        int subtaskIndex = _subtasks.Collection.IndexOf(subtask);
        if (subtaskIndex < _subtasks.Collection.Count - 1 && subtaskIndex != -1)
        {
            _subtasks.Collection.Move(subtaskIndex, subtaskIndex + 1);
        }
    }
}
