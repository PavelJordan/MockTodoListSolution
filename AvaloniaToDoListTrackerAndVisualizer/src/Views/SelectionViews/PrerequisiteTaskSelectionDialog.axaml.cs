using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views;

public partial class PrerequisiteTaskSelectionDialog : Window
{
    public PrerequisiteTaskSelectionDialog()
    {
        InitializeComponent();
        
        if (Design.IsDesignMode)
        {
            return;
        }
        
        RegisterToEvents();

        Closed += (o, e) => { UnregisterFromEvents(); };
        
    }

    private void UnregisterFromEvents()
    {
        WeakReferenceMessenger.Default.Unregister<ClosePrerequisiteSelectionMessage>(this);
    }

    private void RegisterToEvents()
    {
        WeakReferenceMessenger.Default.Register<PrerequisiteTaskSelectionDialog, ClosePrerequisiteSelectionMessage>(this,
            static (window, message) =>
            {
                window.Close(message.ResultPrerequisites);
            });
    }
}
