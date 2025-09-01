using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views;

public partial class PrerequisiteTaskSelectionDialog : Window
{
    public PrerequisiteTaskSelectionDialog()
    {

        WeakReferenceMessenger.Default.Register<PrerequisiteTaskSelectionDialog, ClosePrerequisiteSelectionMessage>(this,
            static (window, message) =>
            {
                window.Close(message.ResultPrerequisites);
            });

        Closed += (o, e) =>
        {
            WeakReferenceMessenger.Default.Unregister<ClosePrerequisiteSelectionMessage>(this);
        };
        
        InitializeComponent();
    }
}
