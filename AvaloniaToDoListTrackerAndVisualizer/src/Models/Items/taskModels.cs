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
/// Abstract class for both regular task (with group assigned, with prerequisites) and subtasks (which can be recursive).
/// Everything is observable to reduce boilerplate by not repeating everything in ViewModel.
/// Subtask class is nested inside TaskBaseModel to access the private AddSubtask method.
/// All property modification must happen on the UI thread!!!
/// </summary>
public abstract partial class TaskBaseModel: ObservableObject, ICompletable, IHasId
{
    
    /// <summary>
    /// For persistent storage purposes
    /// TODO load id from persistent storage next time
    /// </summary>
    public Guid Id { get; } =  Guid.NewGuid();
    
    [ObservableProperty]
    private string _name;
    
    [ObservableProperty]
    private string? _description;
    
    /// <summary>
    /// Also affects Ready property, but that is handled in TaskModel as Ready is not reachable from here.
    /// TODO don't forget to implement the invariant!!!
    /// </summary>
    [ObservableProperty]
    private bool _isCompleted = false;
    
    // TODO - validation for valid dates (begin <= deadlines, etc)
    
    [ObservableProperty]
    private DateTime? _beginDate;
    
    [ObservableProperty]
    private DateTime? _softDeadline;
    
    [ObservableProperty]
    private DateTime? _hardDeadline;
    
    // TODO - Validate to not be negative, aggregate from subtasks
    
    [ObservableProperty]
    private TimeSpan _timeSpent = TimeSpan.Zero;
    
    [ObservableProperty]
    private TimeSpan? _timeExpected;
    
    private readonly ObservableCollection<SubTaskModel> _subtasks = new();
    
    /// <summary>
    /// Invariant - subtask can be not completed only if the base task is also not completed
    /// // TODO implement that invariant (not needed now). MUST NOT BE CYCLIC
    /// </summary>
    public readonly ReadOnlyObservableCollection<SubTaskModel> Subtasks;
    

    protected TaskBaseModel(string name)
    {
        _name = name;
        Subtasks = new (_subtasks);
    }
    
    protected TaskBaseModel(string name, Guid id) : this(name)
    {
        Id = id;
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
        _subtasks.Add(subtask);
    }
    
    public abstract bool CanChangeCompleteness { get; }
}

/// <summary>
/// The SubTask class. Each task can be subtask of only one task and needs to be assigned on its creation.
/// Cannot be not completed, unless the parent is also not completed. Can be recursive
/// </summary>
public sealed partial class SubTaskModel : TaskBaseModel
{
    /// <summary>
    /// Create new subtask for subtaskFor task. Returns the new subtask
    /// </summary>
    public SubTaskModel(string name) : this(name, Guid.NewGuid())
    { }
    
    public SubTaskModel(string name, Guid id) : base(name, id)
    { }

    public override bool CanChangeCompleteness { get; } = true; // TODO manage correctly
}

/// <summary>
/// Represents main task that is not subtask of anything - can be assigned to a group for color,
/// can have prerequisites as other top-leve tasks and can also have preceding events that can block it.
/// Can be not ready yet for completion, but that is not guarded yet (TODO? guard completed only if ready beforehand).
/// Don't forget that TaskModel should be disposed.
/// </summary>
public sealed partial class TaskModel : TaskBaseModel, IDisposable
{
    [ObservableProperty]
    private Group? _group;
    
    // TODO Enforce DAG!!! (no cycles, self-referencing...)
    
    public ObservableChildrenCollectionWrapper<TaskModel> Prerequisites { get; } = new();
    
    public ObservableChildrenCollectionWrapper<PrecedingEvent> PrecedingEvents { get; } = new();
    
    public bool Ready => Prerequisites.Collection.All(x => x.IsCompleted) && PrecedingEvents.Collection.All(x => x.IsCompleted) && !IsCompleted;
    
    public TaskModel(string name): this(name, Guid.NewGuid())
    { }

    public TaskModel(string name, Guid id): base(name, id)
    {
        Prerequisites.Collection.CollectionChanged += OnCollectionChange;
        PrecedingEvents.Collection.CollectionChanged += OnCollectionChange;
        Prerequisites.ChildrenPropertyChanged += OnChildChange;
        PrecedingEvents.ChildrenPropertyChanged += OnChildChange;

        PropertyChanged += CheckIfPropertiesShouldChange;
    }

    /// <summary>
    /// If "IsCompleted" property change, Ready and CanChangeCompleteness can change too. Notify subscribers
    /// </summary>
    private void CheckIfPropertiesShouldChange(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsCompleted))
        {
            OnPropertyChanged(nameof(Ready));
        }

        if (e.PropertyName == nameof(Ready))
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
            return Ready || IsCompleted; // TODO manage correctly (what if future tasks are already completed?)
        }
    }
    
}

public class SaveAbleBaseTask
{
    public required Guid Id { get; set; } 
    public required string Name { get; set; } 
    public string? Description { get; set; }
    public required bool IsCompleted { get; set; }
    public DateTime? BeginDate { get; set; }
    public DateTime? SoftDeadline { get; set; }
    public DateTime? HardDeadline { get; set; }
    public required TimeSpan TimeSpent { get; set; }
    public TimeSpan? TimeExpected { get; set; }
    public required HashSet<Guid> SubtasksIds { get; set; }
}

public class SaveAbleTask: SaveAbleBaseTask
{
    public Guid? GroupId { get; set; }
    public required HashSet<Guid> PrerequisitesIds { get; set; }
    // TODO IMPLEMENT
    //public HashSet<Guid> PrecedingEventsIds { get; set; } = taskModel.PrecedingEvents.Collection.Select(x => x.Id).ToHashSet();
    

    public static SaveAbleTask GetSaveAbleTask(TaskModel taskModel)
    {
        SaveAbleTask result = new SaveAbleTask
        {
            Id = taskModel.Id,
            GroupId = taskModel.Group?.Id,
            PrerequisitesIds = taskModel.Prerequisites.Collection.Select(x => x.Id).ToHashSet(),
            Name = taskModel.Name,
            Description = taskModel.Description,
            IsCompleted = taskModel.IsCompleted,
            BeginDate = taskModel.BeginDate,
            SoftDeadline = taskModel.SoftDeadline,
            HardDeadline = taskModel.HardDeadline,
            TimeSpent = taskModel.TimeSpent,
            TimeExpected = taskModel.TimeExpected,
            SubtasksIds = taskModel.Subtasks.Select(x => x.Id).ToHashSet(),
        };
        return result;
    }

    public static IEnumerable<TaskModel> LinkSaveAbleTasksAndSubtasks(IEnumerable<SaveAbleTask> tasks, IEnumerable<SaveAbleSubTask> subtasks)
    {
        Dictionary<Guid, TaskModel> taskDictionary = new();
        
        var saveAbleTasksList = tasks.ToList();
        
        // TODO Create dictionary of subtasks
        
        foreach (var task in saveAbleTasksList)
        {
            TaskModel newTask = new TaskModel(task.Name, task.Id);
            newTask.Description = task.Description;
            newTask.IsCompleted = task.IsCompleted;
            newTask.BeginDate = task.BeginDate;
            newTask.SoftDeadline = task.SoftDeadline;
            newTask.HardDeadline = task.HardDeadline;
            newTask.SoftDeadline = task.SoftDeadline;
            newTask.HardDeadline = task.HardDeadline;
            newTask.TimeSpent = task.TimeSpent;
            newTask.TimeExpected = task.TimeExpected;
            // TODO link subtasks
            taskDictionary.Add(task.Id, newTask);
        }
        
        // TODO link subtasks to each other

        foreach (var task in saveAbleTasksList)
        {
            foreach (var prerequisiteId in task.PrerequisitesIds)
            {
                taskDictionary[task.Id].Prerequisites.Collection.Add(taskDictionary[prerequisiteId]);
            }
        }
        
        // TODO link events

        return taskDictionary.Values;
    } 
}

public class SaveAbleSubTask: SaveAbleBaseTask
{
    public static SaveAbleSubTask GetSaveAbleSubTask(SubTaskModel subTaskModel)
    {
        SaveAbleSubTask result = new SaveAbleSubTask()
        {
            Id = subTaskModel.Id,
            Name = subTaskModel.Name,
            Description = subTaskModel.Description,
            IsCompleted = subTaskModel.IsCompleted,
            BeginDate = subTaskModel.BeginDate,
            SoftDeadline = subTaskModel.SoftDeadline,
            HardDeadline = subTaskModel.HardDeadline,
            TimeSpent = subTaskModel.TimeSpent,
            TimeExpected = subTaskModel.TimeExpected,
            SubtasksIds = subTaskModel.Subtasks.Select(x => x.Id).ToHashSet(),
        };
        return result;
    }
}