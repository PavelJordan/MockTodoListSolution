using System.Collections.ObjectModel;

namespace AvaloniaToDoListTrackerAndVisualizer.Extensions;

public static class ObservableCollectionExtension
{
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