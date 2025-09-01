using System;
using System.ComponentModel.DataAnnotations;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

/// <summary>
/// Represents a group of tasks and subgroups. Can have color assigned.
/// Task assignment to group is inside task class. Each group can be
/// a subgroup of only one group, and each group can have multiple subgroups.
/// </summary>
public partial class Group: ObservableValidator, IHasId
{
    public Guid Id { get; }

    // TODO remove coupling with avalonia
    [ObservableProperty]
    private Color _groupColor;
    
    [ObservableProperty]
    [Required]
    private string _groupName;
    
    // TODO subgroups (not in specification)

    public Group(string groupName, Color groupColor) :  this(groupName, groupColor, Guid.NewGuid())
    { }

    public Group(string groupName, Color groupColor, Guid id)
    {
        _groupColor = groupColor;
        _groupName = groupName;
        Id = id;
    }
}

