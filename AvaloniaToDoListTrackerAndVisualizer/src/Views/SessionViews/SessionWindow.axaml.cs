using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using AvaloniaToDoListTrackerAndVisualizer.Views.SelectionViewModels;
using AvaloniaToDoListTrackerAndVisualizer.Views.SessionViews;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views;

public partial class SessionWindow : Window
{
    public Window? WindowToGoBackTo { get; }
    
    public SessionWindow(Window windowToGoBackTo)
    {
        InitializeComponent();
        
        WindowToGoBackTo = windowToGoBackTo;
        
        RegisterToEvents();

        Closed += (sender,  e) =>
        {
            UnregisterFromEvents();
            if (DataContext is SessionViewModel sessionViewModel)
            {
                // Dispose
                sessionViewModel.Timer.Stop();
            }
            WindowToGoBackTo!.Show();
        };
        
    }

    private void UnregisterFromEvents()
    {
        WeakReferenceMessenger.Default.Unregister<SessionTaskSelectionRequest>(this);
        WeakReferenceMessenger.Default.Unregister<EditTaskInSessionMessage>(this);
        WeakReferenceMessenger.Default.Unregister<SetupTimerRequest>(this);
    }

    private void RegisterToEvents()
    {
        WeakReferenceMessenger.Default.Register<SessionWindow, SessionTaskSelectionRequest>(this,
            static (window, message) =>
            {
                var dialogWindow = new SessionTaskSelectionDialog()
                {
                    DataContext = new SessionTaskSelectionViewModel(message.AllTasks)
                };
                message.Reply(dialogWindow.ShowDialog<TaskViewModel?>(window));
            });
        
        WeakReferenceMessenger.Default.Register<SessionWindow, EditTaskInSessionMessage>(this,
            static void (window, message) =>
            {
                var dialogWindow = new TaskEditView()
                {
                    DataContext = new TaskEditViewModel(message.TaskToEdit, false, message.AllTasks)
                };
                
                if (window.DataContext is SessionViewModel sessionViewModel)
                {
                    dialogWindow.ShowDialog(window).ContinueWith(_ => sessionViewModel.EnsureSelectedTaskIsValid());
                }
            });
        
        WeakReferenceMessenger.Default.Register<SessionWindow, SetupTimerRequest>(this,
            static void (window, message) =>
            {
                var dialogWindow = new TimerSelectionDialog()
                {
                    DataContext = message.Timer
                };
                
                dialogWindow.ShowDialog(window);
            });
    }

    /// <summary>
    /// For design mode
    /// </summary>
    public SessionWindow()
    {
        
    }
}
