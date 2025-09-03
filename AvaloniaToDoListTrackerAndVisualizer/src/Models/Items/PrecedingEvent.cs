using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaToDoListTrackerAndVisualizer.Models;

// TODO implement (not yet used - marked as coming soon)

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
    
    /// <summary>
    /// Event can always change completeness as it cannot have prerequisites or start date or anything.
    /// </summary>
    public bool CanChangeCompleteness { get; } = true;
}
