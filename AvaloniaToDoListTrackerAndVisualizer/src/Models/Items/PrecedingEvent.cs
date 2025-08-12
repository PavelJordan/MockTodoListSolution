using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

public partial class PrecedingEvent(string name): ObservableObject, ICompletable, IHasID
{
    public Guid ID { get; } = Guid.NewGuid();
    
    [ObservableProperty]
    private bool _isCompleted = false;
    
    [ObservableProperty]
    private string _name = name;
}
