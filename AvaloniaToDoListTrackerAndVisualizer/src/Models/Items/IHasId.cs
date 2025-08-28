using System;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

/// <summary>
/// Wherever an object has id for persistent storage purposes.
/// You should always call Guid.NewGuid as initializer.
/// </summary>
public interface IHasId
{
    public Guid Id { get; } 
}
