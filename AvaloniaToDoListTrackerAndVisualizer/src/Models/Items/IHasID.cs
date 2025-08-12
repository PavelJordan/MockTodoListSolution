using System;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

public interface IHasID
{
    public Guid ID { get; }
}
