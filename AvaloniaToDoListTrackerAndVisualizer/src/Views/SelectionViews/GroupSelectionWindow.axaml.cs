using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views;

public partial class GroupSelectionWindow : Window
{
    public GroupSelectionWindow()
    {
        InitializeComponent();
        
        WeakReferenceMessenger.Default.Register<GroupSelectionWindow, CloseGroupSelection>(this, static (window, message) =>
            {
                window.Close(message.GroupToSelect);
            });

        Closed += (sender, e) =>
        {
            WeakReferenceMessenger.Default.Unregister<CloseGroupSelection>(this);
        };
    }
}