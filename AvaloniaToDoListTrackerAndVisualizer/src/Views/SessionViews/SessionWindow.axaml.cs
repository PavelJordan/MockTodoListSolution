using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views;

public partial class SessionWindow : Window
{
    public Window? WindowToGoBackTo { get; }
    public SessionWindow(Window windowToGoBackTo)
    {
        WindowToGoBackTo = windowToGoBackTo;
        
        WeakReferenceMessenger.Default.Register<SessionWindow, SessionTaskSelectionRequest>(this,
            static (window, message) =>
            {
                message.Reply((TaskViewModel?)null);
            });

        Closed += (sender,  e) =>
        {
            WeakReferenceMessenger.Default.Unregister<SessionTaskSelectionRequest>(this);
            WindowToGoBackTo!.Show();
        };
        
        InitializeComponent();
    }

    public SessionWindow()
    {
        
    }
}