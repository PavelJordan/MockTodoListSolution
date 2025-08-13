using System.ComponentModel;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

/// <summary>
/// Item which can be completed. As it is designed for UI application, it has to be
/// observable
/// </summary>
public interface ICompletable: INotifyPropertyChanged
{
    // TODO consider adding completed by / completed at
    public bool IsCompleted { get; set; }
}
