using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using AvaloniaToDoListTrackerAndVisualizer.Views.SelectionViewModels;
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

        Closed += (sender,  e) =>
        {
            WeakReferenceMessenger.Default.Unregister<SessionTaskSelectionRequest>(this);
            WeakReferenceMessenger.Default.Unregister<EditTaskInSessionMessage>(this);
            WindowToGoBackTo!.Show();
        };
        
        InitializeComponent();
    }

    public SessionWindow()
    {
        
    }
}
