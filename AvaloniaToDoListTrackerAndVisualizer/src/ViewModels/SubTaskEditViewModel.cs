using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class SubTaskEditViewModel(SubTaskViewModel subTaskToEdit): ViewModelBase
{
    public LocalizationProvider Localization { get; } = subTaskToEdit.Localization;
    
    public SubTaskViewModel SubTaskToEdit { get; } = subTaskToEdit;

    [RelayCommand]
    private void Exit()
    {
        WeakReferenceMessenger.Default.Send(new CloseSubTaskEditMessage());
    }
}
