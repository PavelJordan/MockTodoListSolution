using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaToDoListTrackerAndVisualizer.Models;

public enum InterfaceColorScheme
{
    Light, Dark, Red, Green, Blue, Yellow
}

/// <summary>
/// Presents personalization options for the user like username, interface color, etc.
/// Changed should happen only on the UI thread.
/// </summary>
public partial class UserSettings: ObservableObject
{
    [ObservableProperty]
    private string _userName =  "User";

    [ObservableProperty] private InterfaceColorScheme _colorScheme = InterfaceColorScheme.Light;
}
