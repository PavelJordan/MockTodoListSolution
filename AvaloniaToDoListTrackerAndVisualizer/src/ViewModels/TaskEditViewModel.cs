namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class TaskEditViewModel(TaskViewModel taskToEdit): ViewModelBase
{
    public TaskViewModel TaskToEdit { get; } = taskToEdit;
}
