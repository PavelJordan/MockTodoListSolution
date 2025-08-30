using Avalonia.Media;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class GroupSelectionViewModel(GroupListViewModel groups): ViewModelBase
{
    public GroupListViewModel Groups { get; } = groups;

    [RelayCommand]
    private void AssignNoGroup()
    {
        WeakReferenceMessenger.Default.Send(new CloseGroupSelection(null));
    }

    [RelayCommand]
    private void NewGroup()
    {
        Groups.AllGroups.Collection.Add(new Group("New Group", Colors.Gray));
    }

    [RelayCommand]
    private void RemoveGroup(Group groupToDelete)
    {
        Groups.AllGroups.Collection.Remove(groupToDelete);
    }

    [RelayCommand]
    private void SelectGroup(Group groupToSelect)
    {
        WeakReferenceMessenger.Default.Send(new CloseGroupSelection(groupToSelect));
    }
}
