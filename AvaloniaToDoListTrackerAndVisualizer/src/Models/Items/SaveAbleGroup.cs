using System;
using System.Drawing;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

public class SaveAbleGroup
{
    public required Guid Id { get; set; }
    public required int GroupColorHash { get; set; }
    public required string GroupName { get; set; }

    public static SaveAbleGroup GetSaveAbleGroup(Group group)
    {
        return new SaveAbleGroup
        {
            Id = group.Id,
            GroupColorHash = group.GroupColor.ToArgb(),
            GroupName = group.GroupName
        };
    }

    public Group ToGroup()
    {
        return new Group(GroupName, Color.FromArgb(GroupColorHash), Id);
    }
}
