using System.ComponentModel;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

public partial interface ICompletable: INotifyPropertyChanged, INotifyPropertyChanging
{
    public bool IsCompleted { get; set; }
}
