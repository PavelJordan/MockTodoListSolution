using System.ComponentModel;

namespace AvaloniaToDoListTrackerAndVisualizer.Models;

/// <summary>
/// Item which can be completed. As it is designed for UI application, it has to be
/// observable
/// </summary>
public interface ICompletable: INotifyPropertyChanged
{
    // TODO consider adding completed by / completed at (not in specification)
    
    public bool IsCompleted { get; set; }
    
    /// <summary>
    /// Whether item is suitable for changing completeness. It should be always,
    /// if IsCompleted is True. Otherwise, it depends on the concrete example
    /// (maybe start date is not yet here? Or task has prerequisites?).
    /// Is not currently enforced by models themselves.
    /// </summary>
    public bool CanChangeCompleteness { get; }
}
