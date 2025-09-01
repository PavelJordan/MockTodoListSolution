using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
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

        Closing += CheckIfCanClose;
        
        Closed += (sender, e) =>
        {
            WeakReferenceMessenger.Default.Unregister<CloseTaskEditMessage>(this);
            Closing -= CheckIfCanClose;
        };
    }

    private void CheckIfCanClose(object? sender, WindowClosingEventArgs args)
    {
        if (DataContext is SubTaskEditViewModel vm)
        {
            if (!vm.CanExit)
            {
                args.Cancel = true;
            }
        }
    }
}
