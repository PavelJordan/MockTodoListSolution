using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace AvaloniaToDoListTrackerAndVisualizer.Decorators;


public class RecursiveObservableCollectionDecorator<TItems> where TItems: INotifyPropertyChanged
{
    public ObservableCollection<TItems> Collection { get; } = new();
    
    public void Add(TItems item) => Collection.Add(item);
    
    private readonly HashSet<TItems> _wired = new();

    public RecursiveObservableCollectionDecorator()
    {
        Collection.CollectionChanged += OnAllManipulation;
    }

    private void OnManipulation(object? sender, PropertyChangedEventArgs e)
    {
        ChildrenPropertyChanged?.Invoke(this, sender, new PropertyChangedEventArgs(e.PropertyName));
    }

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

    private void Wire(IEnumerable? NewItems)
    {
        foreach (var item in NewItems?.OfType<TItems>() ?? Enumerable.Empty<TItems>())
        {
            _wired.Add(item);
            item.PropertyChanged += OnManipulation;
        }
    }

    private void UnWire(IEnumerable? OldItems)
    {
        foreach (var item in  OldItems?.OfType<TItems>() ?? Enumerable.Empty<TItems>())
        {
            _wired.Remove(item);
            item.PropertyChanged -= OnManipulation;
        }
    }

    private void Rewire()
    {
        foreach (var item in  _wired)
        {
            item.PropertyChanged -= OnManipulation;
        }
        
        _wired.Clear();
        
        Wire(Collection);
    }
    
    public delegate void ChildrenPropertyChangedEventHandler(object? sender, object? child, PropertyChangedEventArgs e);
    
    public event ChildrenPropertyChangedEventHandler? ChildrenPropertyChanged;
}
