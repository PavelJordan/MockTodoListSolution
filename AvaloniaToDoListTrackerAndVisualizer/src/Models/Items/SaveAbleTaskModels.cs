using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

public class SaveAbleTaskBase
{
    public required string Name { get; set; } 
    public string? Description { get; set; }
    public required bool IsCompleted { get; set; }
}

public class SaveAbleSubTask : SaveAbleTaskBase
{
    
}


public class SaveAbleTask: SaveAbleTaskBase
{
    public required Guid Id { get; set; } 
    
    
    public Guid? GroupId { get; set; }
    public required HashSet<Guid> PrerequisitesIds { get; set; }
    
    //public HashSet<Guid> PrecedingEventsIds { get; set; } = taskModel.PrecedingEvents.Collection.Select(x => x.Id).ToHashSet();
    
    public DateTime? BeginDate { get; set; }
    public DateTime? SoftDeadline { get; set; }
    public DateTime? HardDeadline { get; set; }
    public required TimeSpan TimeSpent { get; set; }
    public TimeSpan? TimeExpected { get; set; }

    public required IEnumerable<SaveAbleSubTask> SubTasks { get; set; }

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
            SubTasks = taskModel.Subtasks.Select(x => new SaveAbleSubTask(){Name = x.Name, Description = x.Description, IsCompleted = x.IsCompleted})
        };
        
        return result;
    }
}
