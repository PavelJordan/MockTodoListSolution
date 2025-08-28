using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.Views;

public partial class TaskEditView : Window
{
    public TaskEditView()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<TaskEditView, CloseTaskEditMessage>(this, static (window, message) =>
        {
            window.Close(((TaskEditViewModel?)window.DataContext)?.TaskToEdit);
        });

        Closing += (sender, e) =>
        {
            if (DataContext is not null && !((TaskEditViewModel)DataContext).CanCloseAndSave && !((TaskEditViewModel)DataContext).NewTask)
            {
                e.Cancel = true;
            }
        };

        Loaded += (sender, e) =>
        {
            NameTextBox.Focus(NavigationMethod.Tab);
        };
    }
}
