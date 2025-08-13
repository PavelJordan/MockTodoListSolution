using System;
using System.Collections.ObjectModel;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

/// <summary>
/// Represents a group of tasks and subgroups. Can have color assigned.
/// Task assignment to group is inside task class. Each group can be
/// a subgroup of only one group, and each group can have multiple subgroups.
/// </summary>
public partial class Group: ObservableObject, IHasId
{
    public Guid Id { get; } =  Guid.NewGuid();

    [ObservableProperty]
    private Color _groupColor;
    
    private readonly ObservableCollection<Group> _subGroups = new();
    public readonly ReadOnlyObservableCollection<Group> SubGroups;

    private Group? _parentGroup;

    public Group(Color groupColor)
    {
        _groupColor = groupColor;
        SubGroups = new (_subGroups);
    }

    /// <summary>
    /// Add newSubGroup to this group as subgroup
    /// </summary>
    /// <exception cref="InvalidOperationException"> If the subgroup is already subgroup of other group</exception>
    public void AddSubgroup(Group newSubGroup)
    {
        if (newSubGroup._parentGroup is null)
        {
            newSubGroup._parentGroup = this;
            _subGroups.Add(newSubGroup);
        }
        else
        {
            throw new InvalidOperationException("Subgroup is already subgroup of other groups");
        }
    }
    
    /// <summary>
    /// Remove subgroup from this group.
    /// </summary>
    /// <exception cref="InvalidOperationException"> If the subgroup is subgroup of other group </exception>
    public void RemoveSubgroup(Group oldSubGroup)
    {
        if (oldSubGroup._parentGroup != this)
        {
            throw new InvalidOperationException("Subgroup is subgroup of other group");
        }
        oldSubGroup._parentGroup = null;
        _subGroups.Remove(oldSubGroup);
    }
}
