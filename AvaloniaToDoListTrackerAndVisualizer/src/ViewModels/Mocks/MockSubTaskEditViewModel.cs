using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Providers;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class MockSubTaskEditViewModel: SubTaskEditViewModel
{
    public MockSubTaskEditViewModel(): base(new SubTaskViewModel(new SubTaskModel("Test task"), new LocalizationProvider()))
    {
        
    }
}
