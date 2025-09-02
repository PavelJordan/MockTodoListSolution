using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;

namespace AvaloniaToDoListTrackerAndVisualizer.Models;

/// <summary>
/// Not for long term storage
/// </summary>
public record struct TaskApplicationState
{
    public IEnumerable<TaskModel> Tasks { get; }
    public IEnumerable<Group> Groups { get; }
    
    public IEnumerable<Session> Sessions { get; }

    public TaskApplicationState(IEnumerable<TaskModel> tasks, IEnumerable<Group> groups, IEnumerable<Session> sessions)
    {
        Tasks = tasks;
        Groups = groups;
        Sessions = sessions;
    }

    public static TaskApplicationState CreateApplicationStateFromSaveAble(IEnumerable<SaveAbleTask> saveAbleTasks, IEnumerable<SaveAbleGroup> saveAbleGroups, IEnumerable<Session> sessions)
    {
        Dictionary<Guid, Group> groupDictionary = new();
        foreach (var group in saveAbleGroups)
        {
            groupDictionary[group.Id] = group.ToGroup();
        }
        return new TaskApplicationState(LinkSaveAbleTasks(saveAbleTasks, groupDictionary), groupDictionary.Values, sessions);
    }
    
    private static IEnumerable<TaskModel> LinkSaveAbleTasks(IEnumerable<SaveAbleTask> tasks, Dictionary<Guid, Group> groupsDictionary)
    {
        Dictionary<Guid, TaskModel> taskDictionary = new();
        
        var saveAbleTasksList = tasks.ToList();
        
        
        foreach (var task in saveAbleTasksList)
        {
            TaskModel newTask = new TaskModel(task.Name, task.Id)
            {
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                BeginDate = task.BeginDate,
                SoftDeadline = task.SoftDeadline,
                HardDeadline = task.HardDeadline,
                TimeSpent = task.TimeSpent,
                TimeExpected = task.TimeExpected,
                Group = task.GroupId is Guid groupId ? groupsDictionary[groupId] : null
            };
            foreach (var subTask in task.SubTasks)
            {
                newTask.AddSubtask(new SubTaskModel(subTask.Name)
                {
                    Description = subTask.Description,
                    IsCompleted = subTask.IsCompleted,
                });
            }
            taskDictionary.Add(task.Id, newTask);
        }

        foreach (var task in saveAbleTasksList)
        {
            foreach (var prerequisiteId in task.PrerequisitesIds)
            {
                taskDictionary[task.Id].Prerequisites.Collection.Add(taskDictionary[prerequisiteId]);
            }
        }
        
        return taskDictionary.Values;
    }
}
