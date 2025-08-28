using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;

namespace AvaloniaToDoListTrackerAndVisualizer.Services;

public interface ITaskModelFileService
{
    public Task SaveToFileAsync(IEnumerable<TaskModel> tasksToSave);
    public Task<IEnumerable<TaskModel>?> LoadFromFileAsync();
}

public class TaskModelFileService: ITaskModelFileService
{
    private const string FOLDER_NAME = "ToDoListTrackerAndVisualizer";
    private const string TASK_LIST_JSON_FILE_NAME = "tasks.json";
    
    private static readonly string _folderPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), FOLDER_NAME);
    
    private static readonly string _taskListJsonFilePath = Path.Combine(_folderPath, TASK_LIST_JSON_FILE_NAME);
    
    public async Task SaveToFileAsync(IEnumerable<TaskModel> tasksToSave)
    {
        Directory.CreateDirectory(_folderPath);

        await using var fs = File.Create(_taskListJsonFilePath);
        await JsonSerializer.SerializeAsync(fs, tasksToSave.Select(SaveAbleTask.GetSaveAbleTask));
    }

    public async Task<IEnumerable<TaskModel>?> LoadFromFileAsync()
    {
        try
        {
            await using var fs = File.OpenRead(_taskListJsonFilePath);
            return SaveAbleTask.LinkSaveAbleTasks(await JsonSerializer.DeserializeAsync<IEnumerable<SaveAbleTask>>(fs) ?? Enumerable.Empty<SaveAbleTask>());
        }
        catch (Exception e) when (e is IOException or DirectoryNotFoundException)
        {
            return null;
        }
    }
}
