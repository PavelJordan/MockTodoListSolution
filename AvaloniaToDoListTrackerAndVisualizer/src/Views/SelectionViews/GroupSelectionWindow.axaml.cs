using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views;

public partial class GroupSelectionWindow : Window
{
    public GroupSelectionWindow()
    {
        InitializeComponent();
        
        WeakReferenceMessenger.Default.Register<GroupSelectionWindow, CloseGroupSelectionMessage>(this, static (window, message) =>
            {
                window.Close(message.GroupToSelect);
            });

        Closed += (sender, e) =>
        {
            WeakReferenceMessenger.Default.Unregister<CloseGroupSelectionMessage>(this);
        };
    }
}