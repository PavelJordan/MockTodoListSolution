using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DynamicData;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class TaskEditViewModel: ViewModelBase, IDisposable
{
    [ObservableProperty] private int? _expectedHoursPicker;

    [ObservableProperty] private int? _expectedMinutesPicker;

    private static bool FalseConstant { get; } = false;
    
    public TaskViewModel TaskToEdit { get; }
    
    public LocalizationProvider Localization { get; }
    
    public TaskListViewModel Tasks { get; }
    
    public bool NewTask { get; }
    
    public string BackButtonText
    {
        get { return NewTask ? Localization.SaveAndBackButton : Localization.GoBackButton; }
    }
    
    public bool CanCloseAndSave
    {
        get
        {
            return !string.IsNullOrEmpty(TaskToEdit.TaskModel.Name);
        }
    }

    [RelayCommand(CanExecute = nameof(CanCloseAndSave))]
    private void SaveAndExit()
    {
        WeakReferenceMessenger.Default.Send(new CloseTaskEditMessage());
    }

    public TaskEditViewModel(TaskViewModel taskToEdit, bool newTask, TaskListViewModel allTasks)
    {
        TaskToEdit = taskToEdit;
        NewTask = newTask;
        Tasks = allTasks;
        
        // Whether window can be closed or task can be saved
        TaskToEdit.TaskModel.PropertyChanged += NotifySaveAndExitChange;
        
        Localization = TaskToEdit.Localization;
        
        if (taskToEdit.TaskModel.TimeExpected is TimeSpan editTaskExpectedTime)
        {
            ExpectedHoursPicker = (int)editTaskExpectedTime.TotalHours;
            ExpectedMinutesPicker = editTaskExpectedTime.Minutes;
        }

        // Expected time picker -> model
        PropertyChanged += UpdateUnderlyingModel;
    }

    /// <summary>
    /// Used to know whether the new task can be saved, or window with existing task closed
    /// </summary>
    private void NotifySaveAndExitChange(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TaskToEdit.TaskModel.Name))
        {
            SaveAndExitCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand]
    private void AddSubtask()
    {
        TaskToEdit.TaskModel.AddSubtask(new SubTaskModel(Localization.NewSubTaskName));
    }

    [RelayCommand]
    private void RemoveSubtask(SubTaskViewModel subTask)
    {
        TaskToEdit.TaskModel.RemoveSubtask(subTask.SubTask);
    }

    public void Dispose()
    {
        TaskToEdit.TaskModel.PropertyChanged -= NotifySaveAndExitChange;
        PropertyChanged -= UpdateUnderlyingModel;
    }
    
    /// <summary>
    /// Upload the picked expected hours and minutes to the underlying model 
    /// </summary>
    private void UpdateUnderlyingModel(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ExpectedHoursPicker) or nameof(ExpectedMinutesPicker))
        {
            if (ExpectedHoursPicker is not null || ExpectedMinutesPicker is not null)
            {
                TaskToEdit.TaskModel.TimeExpected = new TimeSpan(ExpectedHoursPicker ?? 0, ExpectedMinutesPicker ?? 0, 0);
            }
            else
            {
                TaskToEdit.TaskModel.TimeExpected = null;
            }
        }
    }

    /// <summary>
    /// Open dialog to choose prerequisites, and if successful, replace the old prerequisites with the new ones
    /// </summary>
    [RelayCommand]
    private async Task ManagePrerequisitesAsync()
    {
        IEnumerable<TaskModel>? NewPrerequisites = await WeakReferenceMessenger.Default.Send(new PrerequisiteTaskSelectionRequest(TaskToEdit, Tasks));
        if (NewPrerequisites is not null)
        {
            TaskToEdit.TaskModel.Prerequisites.Collection.Clear();
            TaskToEdit.TaskModel.Prerequisites.Collection.AddRange(NewPrerequisites);
        }
    }

    [RelayCommand]
    private void MoveSubtaskUp(SubTaskModel subTask)
    {
        TaskToEdit.TaskModel.MoveSubtaskUp(subTask);
    }
    
    [RelayCommand]
    private void MoveSubtaskDown(SubTaskModel subTask)
    {
        TaskToEdit.TaskModel.MoveSubtaskDown(subTask);
    }

    /// <summary>
    /// Set time pickers to nothing
    /// </summary>
    [RelayCommand]
    private void ResetTime()
    {
        ExpectedHoursPicker = null;
        ExpectedMinutesPicker= null;
    }

    public string SpentTimeText
    {
        get
        {
            return TaskToEdit.TaskModel.TimeSpent.ToString(@"hh\:mm\:ss");
        }
    }
}
