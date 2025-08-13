using System.Collections.ObjectModel;

namespace AvaloniaToDoListTrackerAndVisualizer.Extensions;

public static class ObservableCollectionExtensions
{
    
    /// <summary>
    /// Add item to collection if the item is not yet in the collection.
    /// </summary>
    /// <returns> True if item was added, false if item was already present</returns>
    public static bool AddIfNotExists<T>(this ObservableCollection<T> collection, T item)
    {
        if (collection.Contains(item))
        {
            return false;
        }
        collection.Add(item);
        return true;
    }
}
