using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

/// <summary>
/// Represents a group of tasks. Can have color and name assigned.
/// Task assignment to group is inside TaskModel class.
/// </summary>
public partial class Group: ObservableValidator, IHasId
{
    /// <summary>
    /// For persistent storage - SaveAbleTaskModels only remember this id
    /// </summary>
    public Guid Id { get; }

    [ObservableProperty]
    private uint _argb;
    
    [ObservableProperty]
    [Required] // TODO actually enforce required in app
    private string _groupName;
    
    // TODO subgroups (not in specification)

    /// <summary>
    /// Construct new group with new id
    /// </summary>
    public Group(string groupName, uint groupColor) :  this(groupName, groupColor, Guid.NewGuid())
    { }

    /// <summary>
    /// Reconstruct old group with specified id
    /// </summary>
    public Group(string groupName, uint argb, Guid id)
    {
        _argb = argb;
        _groupName = groupName;
        Id = id;
    }
}
