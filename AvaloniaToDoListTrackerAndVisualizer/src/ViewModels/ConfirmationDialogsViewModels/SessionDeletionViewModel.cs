using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class SessionDeletionViewModel(LocalizationProvider localization): ViewModelBase
{
    public LocalizationProvider Localization { get; } = localization;
    
    [RelayCommand]
    private void Yes()
    {
        WeakReferenceMessenger.Default.Send(new CloseSessionDeletionMessage(true));
    }

    [RelayCommand]
    private void No()
    {
        WeakReferenceMessenger.Default.Send(new CloseSessionDeletionMessage(false));
    }
}
