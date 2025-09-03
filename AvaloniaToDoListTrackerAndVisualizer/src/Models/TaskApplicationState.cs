using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;

namespace AvaloniaToDoListTrackerAndVisualizer.Models;

/// <summary>
/// Task application state is used for persistent storage - one-time loading/saving.
/// Use the constructor to initialize it and file service can then read contents and save them.
/// Or use static CreateApplicationStateFromSaveAble method to create correctly linked TaskApplicationState and
/// ready to use tasks, groups, sessions, and user settings from contents of persistent files.
/// </summary>
public record struct TaskApplicationState
{
    public IEnumerable<TaskModel> Tasks { get; }
    public IEnumerable<Group> Groups { get; }
    public IEnumerable<Session> Sessions { get; }
    public UserSettings UserSettings { get; }

    /// <summary>
    /// Initialize the task application state with the things you want to save (later give this instance to some file service)
    /// </summary>
    public TaskApplicationState(IEnumerable<TaskModel> tasks, IEnumerable<Group> groups, IEnumerable<Session> sessions, UserSettings userSettings)
    {
        Tasks = tasks;
        Groups = groups;
        Sessions = sessions;
        UserSettings = userSettings;
    }

    /// <summary>
    /// With parameters as items that can be saved/loaded from files directly, create the main Models,
    /// correctly link them and return TaskApplicationState for you to load them up and start your application.
    /// </summary>
    public static TaskApplicationState CreateApplicationStateFromSaveAble(IEnumerable<SaveAbleTask> saveAbleTasks, IEnumerable<SaveAbleGroup> saveAbleGroups, IEnumerable<Session> sessions, UserSettings userSettings)
    {
        Dictionary<Guid, Group> groupDictionary = new();
        foreach (var group in saveAbleGroups)
        {
            groupDictionary[group.Id] = group.ToGroup();
        }
        return new TaskApplicationState(LinkSaveAbleTasks(saveAbleTasks, groupDictionary), groupDictionary.Values, sessions, userSettings);
    }
    
    /// <summary>
    /// With ready groups, convert ids of groups in tasks to those groups and also convert ids in task for prerequisites to other tasks.
    /// </summary>
    private static IEnumerable<TaskModel> LinkSaveAbleTasks(IEnumerable<SaveAbleTask> tasks, Dictionary<Guid, Group> groupsDictionary)
    {
        // So we can later link tasks as prerequisites to each other based on ids
        Dictionary<Guid, TaskModel> taskDictionary = new();
        
        // tasks is IEnumerable - we need to go through it twice, so save it to list
        var saveAbleTasksList = tasks.ToList();
        
        // First create task models of tasks, assign them groups by ids and save them to taskDictionary.
        foreach (var task in saveAbleTasksList)
        {
            TaskModel newTask = task.ToTaskModel();
            newTask.Group = task.GroupId is Guid groupId ? groupsDictionary[groupId] : null;
            taskDictionary.Add(task.Id, newTask);
        }

        // With taskDictionary ready, convert ids for prerequisites in tasks to real tasks.
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
