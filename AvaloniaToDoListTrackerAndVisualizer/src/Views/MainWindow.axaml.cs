using Avalonia.Controls;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
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
    }
}
