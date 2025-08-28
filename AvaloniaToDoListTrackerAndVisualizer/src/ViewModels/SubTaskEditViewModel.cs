using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class SubTaskEditViewModel(LocalizationProvider localization, SubTaskViewModel subTaskToEdit): ViewModelBase
{
    public LocalizationProvider Localization { get; } = localization;
    
    private SubTaskViewModel SubTaskToEdit { get; } = subTaskToEdit;

    [RelayCommand]
    private void Exit()
    {
        
    }
}
