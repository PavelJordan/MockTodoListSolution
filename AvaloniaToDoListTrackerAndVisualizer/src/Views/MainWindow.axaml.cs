using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
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
                DataContext = new TaskEditViewModel(message.TaskToEdit, message.NewTask)
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
        
        Closed += (sender, e) =>
        {
            WeakReferenceMessenger.Default.Unregister<CloseTaskEditMessage>(this);
            WeakReferenceMessenger.Default.Unregister<OpenGroupSelectionRequest>(this);
        };
    }
}
