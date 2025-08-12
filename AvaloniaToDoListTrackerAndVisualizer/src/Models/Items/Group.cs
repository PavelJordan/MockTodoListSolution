using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

public partial class Group: ObservableObject, IHasID
{
    public Guid ID { get; } = Guid.NewGuid();
    
    [ObservableProperty]
    private ObservableCollection<TaskModel> _items = new();
    
    [ObservableProperty]
    private ObservableCollection<Group> _subGroups = new();
    
    [ObservableProperty]
    private Group? _parent;
}
