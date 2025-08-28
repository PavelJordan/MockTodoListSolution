using System.Threading.Tasks;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Providers;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class MockTaskEditViewModel: TaskEditViewModel
{
    public MockTaskEditViewModel() : base(new TaskViewModel(new TaskModel("Hello, World!") { Description = "Some basic description. \n Hello!" }, new LocalizationProvider()), true)
    {
        TaskToEdit.TaskModel.AddSubtask(new SubTaskModel("Test Subtask 1"));
        TaskToEdit.TaskModel.AddSubtask(new SubTaskModel("Test Subtask 2"));
        TaskToEdit.TaskModel.AddSubtask(new SubTaskModel("Test Subtask 3"));
        TaskToEdit.TaskModel.AddSubtask(new SubTaskModel("Test Subtask 4"));
        TaskToEdit.TaskModel.AddSubtask(new SubTaskModel("Test Subtask 5"));
        TaskToEdit.TaskModel.AddSubtask(new SubTaskModel("Test Subtask 6"));
    }
}
