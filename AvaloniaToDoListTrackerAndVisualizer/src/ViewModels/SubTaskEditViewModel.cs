using System;
using System.ComponentModel;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using CommunityToolkit.Mvvm.ComponentModel;
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
