using System;
using System.ComponentModel;
using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class TaskEditViewModel: ViewModelBase
{
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
        WeakReferenceMessenger.Default.Send<CloseEditMessage>(new CloseEditMessage());
    }

    public TaskEditViewModel(TaskViewModel taskToEdit, bool newTask)
    {
        TaskToEdit = taskToEdit;
        NewTask = newTask;
        TaskToEdit.TaskModel.PropertyChanged += NotifySaveAndExitChange;
        Localization = TaskToEdit.Localization;
    }

    private void NotifySaveAndExitChange(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TaskToEdit.TaskModel.Name))
        {
            SaveAndExitCommand.NotifyCanExecuteChanged();
        }
    }
}
