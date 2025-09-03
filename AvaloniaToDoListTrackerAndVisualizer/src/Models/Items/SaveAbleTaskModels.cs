using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaToDoListTrackerAndVisualizer.Models;

/// <summary>
/// Task which can be saved (is very similar to regular TaskBaseModel class, but has no observable properties
/// and more unserializable things). Use classes tha derive from this class to convert
/// back and forth beween regular and SaveAble versions.
/// </summary>
public class SaveAbleTaskBase
{
    public required string Name { get; set; } 
    public string? Description { get; set; }
    public required bool IsCompleted { get; set; }
}

/// <summary>
/// Subtask which can be saved (is very similar to regular Subtask class, but has no observable properties
/// and more unserializable things). Use methods in this class to convert between each other.
/// </summary>
public class SaveAbleSubTask : SaveAbleTaskBase
{
    /// <summary>
    /// Copy subtask contents to SaveAble subtask to later serialize it.
    /// </summary>
    public static SaveAbleSubTask GetSaveAbleSubtask(SubTaskModel subtask)
    {
        return new SaveAbleSubTask
        {
            Name = subtask.Name,
            Description = subtask.Description,
            IsCompleted = subtask.IsCompleted,
        };
    }

    /// <summary>
    /// Reconstruct the subtask from this SaveAbleSubtasks contents
    /// </summary>
    /// <returns></returns>
    public SubTaskModel ToSubTaskModel()
    {
        return new SubTaskModel(Name)
        {
            Description = Description,
            IsCompleted = IsCompleted,
        };
    }
}

/// <summary>
/// TaskModel which can be saved. Has no observable properties or other unserializable things, and
/// uses ids instead of references to shared parts. Use methods in this class to convert between each other.
/// </summary>
public class SaveAbleTask: SaveAbleTaskBase
{
    public required Guid Id { get; set; } 
    
    public Guid? GroupId { get; set; }
    public required HashSet<Guid> PrerequisitesIds { get; set; }
    
    // TODO event implementation
    //public HashSet<Guid> PrecedingEventsIds { get; set; } = taskModel.PrecedingEvents.Collection.Select(x => x.Id).ToHashSet();
    
    public DateTime? BeginDate { get; set; }
    public DateTime? SoftDeadline { get; set; }
    public DateTime? HardDeadline { get; set; }
    public required TimeSpan TimeSpent { get; set; }
    public TimeSpan? TimeExpected { get; set; }

    public required IEnumerable<SaveAbleSubTask> SubTasks { get; set; }

    /// <summary>
    /// Copy task contents to SaveAbleTask to later serialize it (also convert subtasks to
    /// SaveAble subtasks and prerequisites+group to ids.
    /// </summary>
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
            SubTasks = taskModel.Subtasks.Select(SaveAbleSubTask.GetSaveAbleSubtask)
        };
        
        return result;
    }

    /// <summary>
    /// Reconstruct the task from this SaveAbleTask contents.
    /// Needs still to have group and prerequisites assigned manually later (because of ids).
    /// </summary>
    public TaskModel ToTaskModel()
    {
        var result = new TaskModel(Name, Id)
        {
            Description = Description,
            IsCompleted = IsCompleted,
            BeginDate = BeginDate,
            SoftDeadline = SoftDeadline,
            HardDeadline = HardDeadline,
            TimeSpent = TimeSpent,
            TimeExpected = TimeExpected,
        };
        
        foreach (var subTask in SubTasks)
        {
            result.AddSubtask(subTask.ToSubTaskModel());
        }

        return result;
    }
}
