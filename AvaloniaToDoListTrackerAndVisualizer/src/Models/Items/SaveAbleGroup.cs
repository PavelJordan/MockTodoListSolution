using System;

namespace AvaloniaToDoListTrackerAndVisualizer.Models;

/// <summary>
/// Group which can be saved (is very similar to regular Group class, but has no observable properties
/// and more unserializable things). Use methods in this class to convert between each other.
/// </summary>
public class SaveAbleGroup
{
    public required Guid Id { get; set; }
    public required uint GroupColorHash { get; set; }
    public required string GroupName { get; set; }

    /// <summary>
    /// Copy group contents to SaveAble group to later serialize it.
    /// </summary>
    public static SaveAbleGroup GetSaveAbleGroup(Group group)
    {
        return new SaveAbleGroup
        {
            Id = group.Id,
            GroupColorHash = group.Argb,
            GroupName = group.GroupName
        };
    }

    /// <summary>
    /// Reconstruct the group from this SaveAbleGroup contents
    /// </summary>
    /// <returns></returns>
    public Group ToGroup()
    {
        return new Group(GroupName, GroupColorHash, Id);
    }
}
