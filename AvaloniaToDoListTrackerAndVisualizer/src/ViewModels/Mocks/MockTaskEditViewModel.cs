using System.Threading.Tasks;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Providers;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class MockTaskEditViewModel: TaskEditViewModel
{
    public MockTaskEditViewModel(): base(new TaskViewModel(new TaskModel("Hello, World!"){Description = "Some basic description. \n Hello!"}, new LocalizationProvider()), true)
    {
        
    }
}
