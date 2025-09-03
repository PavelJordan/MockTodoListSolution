using System.Drawing;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Providers;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class MockGroupSelectionViewModel: GroupSelectionViewModel
{
    public MockGroupSelectionViewModel(): base(new GroupListViewModel(new LocalizationProvider()))
    {
        Groups.AllGroups.Collection.Add(new Group("Hello, World!", (uint)Color.Red.ToArgb()));
        Groups.AllGroups.Collection.Add(new Group("Group 1", (uint)Color.Orange.ToArgb()));
        Groups.AllGroups.Collection.Add(new Group("Group 2", (uint)Color.Lime.ToArgb()));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", (uint)Color.MediumPurple.ToArgb()));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", (uint)Color.MediumPurple.ToArgb()));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", (uint)Color.MediumPurple.ToArgb()));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", (uint)Color.MediumPurple.ToArgb()));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", (uint)Color.MediumPurple.ToArgb()));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", (uint)Color.MediumPurple.ToArgb()));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", (uint)Color.MediumPurple.ToArgb()));
    }
}