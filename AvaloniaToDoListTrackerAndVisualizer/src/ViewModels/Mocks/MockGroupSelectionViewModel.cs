using Avalonia.Media;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Providers;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public class MockGroupSelectionViewModel: GroupSelectionViewModel
{
    public MockGroupSelectionViewModel(): base(new GroupListViewModel(new LocalizationProvider()))
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