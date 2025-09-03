using System;
using System.ComponentModel;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class SubTaskEditViewModel: ViewModelBase, IDisposable
{
    public LocalizationProvider Localization { get; }
    
    public SubTaskViewModel SubTaskToEdit { get; }

    public SubTaskEditViewModel(SubTaskViewModel subTaskToEdit)
    {
        SubTaskToEdit = subTaskToEdit;
        Localization =  subTaskToEdit.Localization;

        // If name was changed, check, if it's not empty. If it is, you cannot close the edit window
        SubTaskToEdit.SubTask.PropertyChanged += CheckIfCanExistChanged;
    }
    
    
    public bool CanExit {
        get
        {
            return SubTaskToEdit.SubTask.Name != string.Empty;
        }
    }

    [RelayCommand(CanExecute = nameof(CanExit))]
    private void Exit()
    {
        WeakReferenceMessenger.Default.Send(new CloseSubTaskEditMessage());
    }

    /// <summary>
    /// If name was changed, check, if it's not empty. If it is, you cannot close the edit window
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckIfCanExistChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SubTaskToEdit.SubTask.Name))
        {
            OnPropertyChanged(nameof(CanExit));
            ExitCommand.NotifyCanExecuteChanged();
        }
    }

    public void Dispose()
    {
        SubTaskToEdit.SubTask.PropertyChanged -= CheckIfCanExistChanged;
    }
}
