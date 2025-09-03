using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views;

public partial class TimerSelectionDialog : Window
{
    public TimerSelectionDialog()
    {
        InitializeComponent();
        
        if (Design.IsDesignMode)
        {
            return;
        }
        
        RegisterToEvents();

        Closing += (obj, args) =>
        {
            if (DataContext is TimerViewModel timer)
            {
                // Timer might now be set to something different - refresh
                timer.RefreshTimerProperties();
            }

            UnregisterFromEvents();
        };
        
    }

    private void UnregisterFromEvents()
    {
        WeakReferenceMessenger.Default.Unregister<CloseTimerSelectionDialogMessage>(this);
    }

    private void RegisterToEvents()
    {
        WeakReferenceMessenger.Default.Register<TimerSelectionDialog, CloseTimerSelectionDialogMessage>(this, static (window, message) =>
        {
            window.Close();
        });
    }
}
