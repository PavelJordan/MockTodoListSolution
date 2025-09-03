using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Providers;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class MockSubTaskEditViewModel: SubTaskEditViewModel
{
    public MockSubTaskEditViewModel(): base(new SubTaskViewModel(new SubTaskModel("Test task"), new LocalizationProvider()))
    {
        
    }
}
