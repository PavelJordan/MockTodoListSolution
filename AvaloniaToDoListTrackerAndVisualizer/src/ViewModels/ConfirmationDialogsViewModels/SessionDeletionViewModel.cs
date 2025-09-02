using AvaloniaToDoListTrackerAndVisualizer.Messages;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class SessionDeletionViewModel: ViewModelBase
{
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
