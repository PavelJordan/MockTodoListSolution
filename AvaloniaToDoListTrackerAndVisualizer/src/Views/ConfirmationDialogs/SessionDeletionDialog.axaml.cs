using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views;

public partial class SessionDeletionDialog : Window
{
    public SessionDeletionDialog()
    {
        InitializeComponent();
        
        if (Design.IsDesignMode)
        {
            return;
        }
        
        RegisterToEvents();

        Closed += (sender, args) => { UnregisterFromEvents(); };
        
    }

    private void UnregisterFromEvents()
    {
        WeakReferenceMessenger.Default.Unregister<CloseSessionDeletionMessage>(this);
    }

    private void RegisterToEvents()
    {
        WeakReferenceMessenger.Default.Register<SessionDeletionDialog, CloseSessionDeletionMessage>(this, static (window, message) =>
        {
            window.Close(message.ShouldDeleteSession);
        });
    }
}
