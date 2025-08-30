using System;
using Avalonia.Media;

namespace AvaloniaToDoListTrackerAndVisualizer.Models.Items;

public class SaveAbleGroup
{
    public required Guid Id { get; set; }
    public required string GroupColorHash { get; set; }
    public required string GroupName { get; set; }

    public static SaveAbleGroup GetSaveAbleGroup(Group group)
    {
        return new SaveAbleGroup
        {
            Id = group.Id,
            GroupColorHash = group.GroupColor.ToString() ?? "#FF858585",
            GroupName = group.GroupName
        };
    }

    public Group ToGroup()
    {
        return new Group(GroupName, Color.Parse(GroupColorHash), Id);
    }
}
