using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views;

public partial class SubTaskEditView : Window
{
    public SubTaskEditView()
    {
        InitializeComponent();
        
        WeakReferenceMessenger.Default.Register<SubTaskEditView, CloseSubTaskEditMessage>(this, static (window, message) =>
        {
            window.Close();
        });
        
        Closed += (sender, e) =>
        {
            WeakReferenceMessenger.Default.Unregister<CloseTaskEditMessage>(this);
        };
    }
}