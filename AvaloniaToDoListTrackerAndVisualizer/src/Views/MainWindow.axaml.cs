using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
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
        
        Closed += (sender, e) =>
        {
            WeakReferenceMessenger.Default.Unregister<EditTaskMessage>(this);
            WeakReferenceMessenger.Default.Unregister<OpenGroupSelectionRequest>(this);
            WeakReferenceMessenger.Default.Unregister<StartSessionMessage>(this);
        };
    }
}
