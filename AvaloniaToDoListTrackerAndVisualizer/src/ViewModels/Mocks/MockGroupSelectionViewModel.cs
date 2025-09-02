using System.Drawing;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Providers;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class MockGroupSelectionViewModel: GroupSelectionViewModel
{
    public MockGroupSelectionViewModel(): base(new GroupListViewModel(new LocalizationProvider()))
    {
        Groups.AllGroups.Collection.Add(new Group("Hello, World!", Color.Red));
        Groups.AllGroups.Collection.Add(new Group("Group 1", Color.Orange));
        Groups.AllGroups.Collection.Add(new Group("Group 2", Color.Lime));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", Color.MediumPurple));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", Color.MediumPurple));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", Color.MediumPurple));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", Color.MediumPurple));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", Color.MediumPurple));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", Color.MediumPurple));
        Groups.AllGroups.Collection.Add(new Group("RepeatGroup", Color.MediumPurple));
    }
}