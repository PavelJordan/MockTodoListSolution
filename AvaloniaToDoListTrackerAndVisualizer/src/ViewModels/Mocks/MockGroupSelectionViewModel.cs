using Avalonia.Media;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class MockGroupSelectionViewModel: GroupSelectionViewModel
{
    public MockGroupSelectionViewModel(): base(new GroupListViewModel())
    {
        Groups.AllGroups.Collection.Add(new Group("Hello, World!", Colors.Red));
        Groups.AllGroups.Collection.Add(new Group("Group 1", Colors.Orange));
        Groups.AllGroups.Collection.Add(new Group("Group 2", Colors.Lime));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", Colors.MediumPurple));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", Colors.MediumPurple));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", Colors.MediumPurple));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", Colors.MediumPurple));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", Colors.MediumPurple));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", Colors.MediumPurple));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", Colors.MediumPurple));
    }
}