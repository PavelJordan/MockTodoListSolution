using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views.SelectionViewModels;

public partial class SessionTaskSelectionDialog : Window
{
    public SessionTaskSelectionDialog()
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
        WeakReferenceMessenger.Default.Unregister<CloseSessionTaskSelectionMessage>(this);
    }

    private void RegisterToEvents()
    {
        WeakReferenceMessenger.Default.Register<SessionTaskSelectionDialog, CloseSessionTaskSelectionMessage>(this,
            static (window, message) =>
            {
                window.Close(message.SelectedTask);
            });
    }
}
