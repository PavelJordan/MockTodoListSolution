using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using AvaloniaToDoListTrackerAndVisualizer.Views.ConfirmationDialogs;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        if (Design.IsDesignMode)
        {
            return;
        }

        WeakReferenceMessenger.Default.Register<MainWindow, EditTaskMessage>(this, static (window, message) =>
        {
            var dialog = new TaskEditView()
            {
                DataContext = new TaskEditViewModel(message.TaskToEdit, message.NewTask, message.AllTasks)
            };
            
            message.Reply(dialog.ShowDialog<TaskViewModel?>(window));
        });
        
        WeakReferenceMessenger.Default.Register<MainWindow, OpenGroupSelectionRequest>(this, static (window, message) =>
        {
            var dialog = new GroupSelectionWindow()
            {
                DataContext = new GroupSelectionViewModel(message.Groups)
            };
            
            message.Reply(dialog.ShowDialog<Group?>(window));
        });

        WeakReferenceMessenger.Default.Register<MainWindow, StartSessionMessage>(this, static (window, message) =>
        {
            window.Hide();
            Session newSession = new Session();
            var sessionWindow = new SessionWindow(window)
            {
                DataContext = new SessionViewModel(message.Tasks, message.Groups, newSession)
            };
            
            if (window.DataContext is MainWindowViewModel vm)
            {
                vm.Sessions.Add(newSession);
            }
            
            sessionWindow.Show();
        });
        
        WeakReferenceMessenger.Default.Register<MainWindow, SessionDeletionRequest>(this, static (window, message) =>
        {
            LocalizationProvider local;
            if (window.DataContext is MainWindowViewModel mainWindowViewModel)
            {
                local = mainWindowViewModel.Localization;
            }
            else
            {
                local = new LocalizationProvider();
            }
            var dialog = new SessionDeletionDialog()
            {
                DataContext = new SessionDeletionViewModel(local)
            };
            message.Reply(dialog.ShowDialog<bool?>(window));
        });
        
        Closed += (sender, e) =>
        {
            WeakReferenceMessenger.Default.Unregister<EditTaskMessage>(this);
            WeakReferenceMessenger.Default.Unregister<OpenGroupSelectionRequest>(this);
            WeakReferenceMessenger.Default.Unregister<StartSessionMessage>(this);
            WeakReferenceMessenger.Default.Unregister<SessionDeletionRequest>(this);
        };
    }
}
