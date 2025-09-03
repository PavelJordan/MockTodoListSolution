using System;

namespace AvaloniaToDoListTrackerAndVisualizer.Models;

/// <summary>
/// Wherever an object has id for persistent storage purposes.
/// You should always call Guid.NewGuid as initializer for new items.
/// </summary>
public interface IHasId
{
    public Guid Id { get; } 
}
