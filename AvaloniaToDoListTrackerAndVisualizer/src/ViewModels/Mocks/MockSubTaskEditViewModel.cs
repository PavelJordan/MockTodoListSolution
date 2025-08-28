using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Providers;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class MockSubTaskEditViewModel: SubTaskEditViewModel
{
    public MockSubTaskEditViewModel(): base(new LocalizationProvider(), new SubTaskViewModel(new SubTaskModel("Test task")))
    {
        
    }
    
}
