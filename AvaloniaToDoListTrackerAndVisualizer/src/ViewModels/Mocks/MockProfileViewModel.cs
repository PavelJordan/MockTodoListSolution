using System;
using System.Collections.ObjectModel;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Providers;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class MockProfileViewModel: ProfileViewModel
{
    private static ObservableCollection<Session>_getDefaultSessions()
    {
        Session s1 = new();
        Session s2 = new();
        Session s3 = new();
        s1.AddPart(new SessionPart(DateTimeOffset.Now.Subtract(TimeSpan.FromMinutes(5)), DateTimeOffset.Now.Add(TimeSpan.FromMinutes(5))));
        s2.AddPart(new SessionPart(DateTimeOffset.Now.Subtract(TimeSpan.FromMinutes(5) + TimeSpan.FromDays(1)), DateTimeOffset.Now.Subtract(TimeSpan.FromDays(1) - TimeSpan.FromMinutes(5))));
        s3.AddPart(new SessionPart(DateTimeOffset.Now.Subtract(TimeSpan.FromMinutes(5) + TimeSpan.FromMinutes(10)), DateTimeOffset.Now.Add(TimeSpan.FromMinutes(5) + TimeSpan.FromMinutes(10))));
        s3.AddPart(new SessionPart(DateTimeOffset.Now.Subtract(TimeSpan.FromMinutes(5) + TimeSpan.FromDays(1) + TimeSpan.FromMinutes(10)), DateTimeOffset.Now.Subtract(TimeSpan.FromDays(1) + TimeSpan.FromMinutes(10) - TimeSpan.FromMinutes(5))));
        ObservableCollection<Session> sessions = new();
        sessions.Add(s1);
        sessions.Add(s2);
        sessions.Add(s3);
        return sessions;
    }

    public MockProfileViewModel() : base(new LocalizationProvider(), _getDefaultSessions(), new UserSettings())
    { } 
}
