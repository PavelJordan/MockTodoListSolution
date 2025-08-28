using System;
using System.ComponentModel;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class TaskEditViewModel: ViewModelBase, IDisposable
{
    [ObservableProperty] private int? _expectedHoursPicker;

    [ObservableProperty] private int? _expectedMinutesPicker;
    
    public TaskViewModel TaskToEdit { get; }
    
    public LocalizationProvider Localization { get; }
    
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

    public TaskEditViewModel(TaskViewModel taskToEdit, bool newTask)
    {
        TaskToEdit = taskToEdit;
        NewTask = newTask;
        TaskToEdit.TaskModel.PropertyChanged += NotifySaveAndExitChange;
        Localization = TaskToEdit.Localization;
        if (taskToEdit.TaskModel.TimeExpected is TimeSpan editTaskExpectedTime)
        {
            ExpectedHoursPicker = (int)editTaskExpectedTime.TotalHours;
            ExpectedMinutesPicker = editTaskExpectedTime.Minutes;
        }

        PropertyChanged += UpdateUnderlyingModel;
    }

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
    
    private void UpdateUnderlyingModel(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ExpectedHoursPicker) or nameof(ExpectedMinutesPicker))
        {
            if (ExpectedHoursPicker is not null || ExpectedMinutesPicker is not null)
            {
                ExpectedHoursPicker ??= 0;
                ExpectedMinutesPicker ??= 0;
                TaskToEdit.TaskModel.TimeExpected = new TimeSpan(ExpectedHoursPicker.Value, ExpectedMinutesPicker.Value, 0);
            }
            else
            {
                TaskToEdit.TaskModel.TimeExpected = null;
            }
        }
    }
}
