using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

/// <summary>
/// Represents an event which can precede some tasks. Tasks can listen to the IsCompleted property.
/// Property changes should occur only on the UI thread.
/// </summary>
/// <param name="name"> Name of the event </param>
public sealed partial class PrecedingEvent(string name): ObservableObject, ICompletable, IHasId
{
    public Guid Id { get; } = Guid.NewGuid();
    
    [ObservableProperty]
    private bool _isCompleted = false;
    
    [ObservableProperty]
    private string _name = name;
    
    // TODO maybe add Equals/GetHashCode overloads for better use
    public bool CanChangeCompleteness { get; } = true;
}
