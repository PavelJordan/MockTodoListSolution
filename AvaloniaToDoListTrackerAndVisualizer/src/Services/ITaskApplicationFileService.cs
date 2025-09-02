using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;

namespace AvaloniaToDoListTrackerAndVisualizer.Services;



public interface ITaskApplicationFileService
{
    public Task<TaskApplicationState?> LoadFromFileAsync();
    public Task SaveToFileAsync(TaskApplicationState applicationStateToSave);
}

public class TaskApplicationFileService: ITaskApplicationFileService
{
    private const string FOLDER_NAME = "ToDoListTrackerAndVisualizer";
    private const string TASK_LIST_JSON_FILE_NAME = "tasks.json";
    private const string GROUP_LIST_JSON_FILE_NAME = "groups.json";
    private const string SESSIONS_JSON_FILE_NAME = "sessions.json";
    private const string USER_SETTINGS_JSON_FILE_NAME = "userSettings.json";
    
    private static readonly string _folderPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), FOLDER_NAME);
    
    private static readonly string _taskListJsonFilePath = Path.Combine(_folderPath, TASK_LIST_JSON_FILE_NAME);
    private static readonly string _groupListJsonFilePath = Path.Combine(_folderPath, GROUP_LIST_JSON_FILE_NAME);
    private static readonly string _sessionsJsonFilePath = Path.Combine(_folderPath, SESSIONS_JSON_FILE_NAME);
    private static readonly string _userSettingsJsonFilePath = Path.Combine(_folderPath, USER_SETTINGS_JSON_FILE_NAME);
    
    public async Task SaveToFileAsync(TaskApplicationState applicationStateToSave)
    {
        Directory.CreateDirectory(_folderPath);

        await using var saveAbleTasksFile = new FileStream(_taskListJsonFilePath, FileMode.Create,  FileAccess.Write,  FileShare.None);
        await using var saveAbleGroupsFile = new FileStream(_groupListJsonFilePath, FileMode.Create,  FileAccess.Write,  FileShare.None);
        await using var sessionsFile = new FileStream(_sessionsJsonFilePath, FileMode.Create,  FileAccess.Write,  FileShare.None);
        await using var userSettingsFile = new FileStream(_userSettingsJsonFilePath, FileMode.Create,  FileAccess.Write,  FileShare.None);
        await JsonSerializer.SerializeAsync(saveAbleTasksFile, applicationStateToSave.Tasks.Select( SaveAbleTask.GetSaveAbleTask));
        await JsonSerializer.SerializeAsync(saveAbleGroupsFile, applicationStateToSave.Groups.Select( SaveAbleGroup.GetSaveAbleGroup));
        await JsonSerializer.SerializeAsync(sessionsFile, applicationStateToSave.Sessions);
        await JsonSerializer.SerializeAsync(userSettingsFile, applicationStateToSave.UserSettings);
    }

    public async Task<TaskApplicationState?> LoadFromFileAsync()
    {
        try
        {
            await using var saveAbleTasksFile = File.OpenRead(_taskListJsonFilePath);
            await using var saveAbleGroupsFile = File.OpenRead(_groupListJsonFilePath);
            await using var sessionsFile = File.OpenRead(_sessionsJsonFilePath);
            await using var userSettingsFile = File.OpenRead(_userSettingsJsonFilePath);
            IEnumerable<SaveAbleTask> saveAbleTasks = await JsonSerializer.DeserializeAsync<IEnumerable<SaveAbleTask>>(saveAbleTasksFile) ??
                                                      Enumerable.Empty<SaveAbleTask>();
            
            IEnumerable<SaveAbleGroup> saveAbleGroups = await JsonSerializer.DeserializeAsync<IEnumerable<SaveAbleGroup>>(saveAbleGroupsFile) ?? Enumerable.Empty<SaveAbleGroup>();
            
            IEnumerable<Session> sessions = await JsonSerializer.DeserializeAsync<IEnumerable<Session>>(sessionsFile) ?? Enumerable.Empty<Session>();
            
            UserSettings userSettings = await JsonSerializer.DeserializeAsync<UserSettings>(userSettingsFile) ?? new UserSettings();
            
            return TaskApplicationState.CreateApplicationStateFromSaveAble(saveAbleTasks, saveAbleGroups, sessions, userSettings);
        }
        catch (Exception e) when (e is IOException or DirectoryNotFoundException)
        {
            return null;
        }
    }
}
