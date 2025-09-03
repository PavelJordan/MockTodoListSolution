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
        
        if (Design.IsDesignMode)
        {
            return;
        }
        
        RegisterToEvents();

        Closing += CheckIfCanClose;
        
        Closed += (sender, e) =>
        {
            UnregisterFromEvents();
            Closing -= CheckIfCanClose;
        };
    }

    private void UnregisterFromEvents()
    {
        WeakReferenceMessenger.Default.Unregister<CloseSubTaskEditMessage>(this);
    }

    private void RegisterToEvents()
    {
        WeakReferenceMessenger.Default.Register<SubTaskEditView, CloseSubTaskEditMessage>(this, static (window, message) =>
        {
            window.Close();
        });
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
