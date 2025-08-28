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
    
    // TODO implement events (in specification)
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

    public static IEnumerable<TaskModel> LinkSaveAbleTasks(IEnumerable<SaveAbleTask> tasks)
    {
        Dictionary<Guid, TaskModel> taskDictionary = new();
        
        var saveAbleTasksList = tasks.ToList();
        
        
        foreach (var task in saveAbleTasksList)
        {
            TaskModel newTask = new TaskModel(task.Name, task.Id);
            newTask.Description = task.Description;
            newTask.IsCompleted = task.IsCompleted;
            newTask.BeginDate = task.BeginDate;
            newTask.SoftDeadline = task.SoftDeadline;
            newTask.HardDeadline = task.HardDeadline;
            newTask.TimeSpent = task.TimeSpent;
            newTask.TimeExpected = task.TimeExpected;
            taskDictionary.Add(task.Id, newTask);
            foreach (var subTask in task.SubTasks)
            {
                newTask.AddSubtask(new SubTaskModel(subTask.Name)
                {
                    Description = subTask.Description,
                    IsCompleted = subTask.IsCompleted,
                });
            }
        }

        foreach (var task in saveAbleTasksList)
        {
            foreach (var prerequisiteId in task.PrerequisitesIds)
            {
                taskDictionary[task.Id].Prerequisites.Collection.Add(taskDictionary[prerequisiteId]);
            }
        }
        
        // TODO link events (in specification)

        return taskDictionary.Values;
    } 
}
