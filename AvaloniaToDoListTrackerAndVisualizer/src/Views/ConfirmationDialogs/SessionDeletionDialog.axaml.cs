using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views.ConfirmationDialogs;

public partial class SessionDeletionDialog : Window
{
    public SessionDeletionDialog()
    {
        WeakReferenceMessenger.Default.Register<SessionDeletionDialog, CloseSessionDeletionMessage>(this, static (window, message) =>
        {
            window.Close(message.ShouldDeleteSession);
        });

        Closed += (sender, args) =>
        {
            WeakReferenceMessenger.Default.Unregister<CloseSessionDeletionMessage>(this);
        };
        
        InitializeComponent();
    }
}
