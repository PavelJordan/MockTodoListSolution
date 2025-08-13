using System;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

/// <summary>
/// Wherever an object has Id for persistent storage purposes.
/// You should always call Guid.NewGuid as initializer
/// </summary>
public interface IHasId
{
    public Guid Id { get; } 
}
