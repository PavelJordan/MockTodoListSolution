using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views;

public partial class GroupSelectionWindow : Window
{
    public GroupSelectionWindow()
    {
        InitializeComponent();
        
        if (Design.IsDesignMode)
        {
            return;
        }
        
        RegisterToEvents();

        Closed += (sender, e) => { UnregisterFromEvents(); };
    }

    private void UnregisterFromEvents()
    {
        WeakReferenceMessenger.Default.Unregister<CloseGroupSelectionMessage>(this);
    }

    private void RegisterToEvents()
    {
        WeakReferenceMessenger.Default.Register<GroupSelectionWindow, CloseGroupSelectionMessage>(this, static (window, message) =>
        {
            window.Close(message.GroupToSelect);
        });
    }
}
