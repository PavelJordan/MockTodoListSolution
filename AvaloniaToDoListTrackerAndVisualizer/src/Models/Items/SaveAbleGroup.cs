using System;
using System.Drawing;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

public class SaveAbleGroup
{
    public required Guid Id { get; set; }
    public required uint GroupColorHash { get; set; }
    public required string GroupName { get; set; }

    public static SaveAbleGroup GetSaveAbleGroup(Group group)
    {
        return new SaveAbleGroup
        {
            Id = group.Id,
            GroupColorHash = group.Argb,
            GroupName = group.GroupName
        };
    }

    public Group ToGroup()
    {
        return new Group(GroupName, GroupColorHash, Id);
    }
}
