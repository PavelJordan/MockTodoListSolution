using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace AvaloniaToDoListTrackerAndVisualizer.Wrappers;

/// <summary>
/// Wrapper around ObservableCollection, which allows you to subscribe to
/// children's property changed events.
/// Must be mutated on the UI thread!
/// </summary>
public class ObservableChildrenCollectionWrapper<TItems>: IDisposable where TItems: INotifyPropertyChanged
{
    public ObservableCollection<TItems> Collection { get; } = new();
    
    /// <summary>
    /// Items that are monitored for property changed events. Used for unsubscribing
    /// if large modifications happen to collection (NotifyCollectionChangedAction.Reset)
    /// </summary>
    private readonly Dictionary<TItems, uint> _wired = new();

    public ObservableChildrenCollectionWrapper()
    {
        Collection.CollectionChanged += OnAllManipulation;
    }

    /// <summary>
    /// Item in collection has its property changed event handler
    /// </summary>
    private void OnManipulation(object? sender, PropertyChangedEventArgs e)
    {
        ChildrenPropertyChanged?.Invoke(this, new(sender, e));
    }

    /// <summary>
    /// Collection changed event handler
    /// </summary>
    private void OnAllManipulation(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Replace)
        {
            UnWire(e.OldItems);
        }
            
        if (e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Replace)
        {
            Wire(e.NewItems);
        }

        if (e.Action is NotifyCollectionChangedAction.Reset)
        {
            Rewire();
        }
    }

    /// <summary>
    /// Subscribe to newItems and add them to wired items
    /// </summary>
    private void Wire(IEnumerable? newItems)
    {
        foreach (var item in newItems?.OfType<TItems>() ?? Enumerable.Empty<TItems>())
        {
            if (_wired.TryAdd(item, 0))
            {
                item.PropertyChanged += OnManipulation;
            }
            _wired[item]++;
        }
    }

    /// <summary>
    /// Unsubscribe from oldItems and remove them from wired items
    /// </summary>
    private void UnWire(IEnumerable? oldItems)
    {
        foreach (var item in  oldItems?.OfType<TItems>() ?? Enumerable.Empty<TItems>())
        {
            _wired[item]--;
            if (_wired[item] == 0)
            {
                _wired.Remove(item);
                item.PropertyChanged -= OnManipulation;
            }
        }
    }

    /// <summary>
    /// Unsubscribe from all wired items (now nothing is subscribed) and subscribe to everything in collection.
    /// </summary>
    private void Rewire()
    {
        UnWireAll();
        Wire(Collection);
    }

    /// <summary>
    /// Unsubscribe from all wired items.
    /// </summary>
    private void UnWireAll()
    {
        foreach (var item in  _wired.Keys)
        {
            item.PropertyChanged -= OnManipulation;
            
        }
        
        _wired.Clear();
    }
    
    /// <summary>
    /// Sender (collection) has its child's (item's) property changed
    /// </summary>
    public event EventHandler<ChildrenPropertyChangedEventArgs>? ChildrenPropertyChanged;
    
    public void Dispose()
    {
        UnWireAll();
        Collection.CollectionChanged -= OnAllManipulation;
    }
}

public class ChildrenPropertyChangedEventArgs(object? child, PropertyChangedEventArgs childrenEventArgs) : EventArgs
{
    public object? Child { get; set; } = child;
    public PropertyChangedEventArgs ChildrenEventArgs { get; set; } = childrenEventArgs;
}
