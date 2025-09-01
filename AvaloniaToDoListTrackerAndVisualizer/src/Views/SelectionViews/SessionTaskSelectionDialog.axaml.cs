using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views.SelectionViewModels;

public partial class SessionTaskSelectionDialog : Window
{
    public SessionTaskSelectionDialog()
    {
        WeakReferenceMessenger.Default.Register<SessionTaskSelectionDialog, CloseSessionTaskSelectionMessage>(this,
            static (window, message) =>
            {
                window.Close(message.SelectedTask);
            });

        Closed += (sender, args) =>
        {
            WeakReferenceMessenger.Default.Unregister<CloseSessionTaskSelectionMessage>(this);
        };
        
        InitializeComponent();
    }
}
