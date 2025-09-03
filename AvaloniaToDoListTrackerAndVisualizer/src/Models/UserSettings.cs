using System;

namespace AvaloniaToDoListTrackerAndVisualizer.Models;

/// <summary>
/// Presents personalization options for the user like username, interface color, etc.
/// Changed should happen only on the UI thread.
/// TODO actually implement the things
/// </summary>
public class UserSettings
{
    public TimeSpan DailyGoal { get; set; }
}
