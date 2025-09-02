using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views.SessionViews;

public partial class TimerSelectionDialog : Window
{
    public TimerSelectionDialog()
    {
        
        WeakReferenceMessenger.Default.Register<TimerSelectionDialog, CloseTimerSelectionDialogMessage>(this, static (window, message) =>
        {
            window.Close();
        });

        Closing += (obj, args) =>
        {
            if (DataContext is TimerViewModel timer)
            {
                timer.RefreshTimerProperties();
            }
            
            WeakReferenceMessenger.Default.Unregister<CloseTimerSelectionDialogMessage>(this);
        };
        
        InitializeComponent();
    }
}