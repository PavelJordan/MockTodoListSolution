using System.ComponentModel;
using System.Globalization;
using AvaloniaToDoListTrackerAndVisualizer.Lang;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaToDoListTrackerAndVisualizer.Providers;

public class LocalizationProvider: ObservableObject
{
    public string AddEventButton => Resources.AddEventButton;
    public string AddTaskButton => Resources.AddTaskButton;
    public string AllButton => Resources.AllButton;
    public string CompleteButton => Resources.CompleteButton;
    public string DetailsButton => Resources.DetailsButton;
    public string DoneButton => Resources.DoneButton;
    public string HomeButton => Resources.HomeButton;
    public string ProfileButton => Resources.ProfileButton;
    public string ReadyButton => Resources.ReadyButton;
    public string SettingsButton => Resources.SettingsButton;
    public string StartSessionButton => Resources.StartSessionButton;
    public string TreeButton => Resources.TreeButton;
    public string UnCompleteButton => Resources.UnCompleteButton;

    public void SetCulture(CultureInfo culture)
    {
        Resources.Culture = culture;
        OnPropertyChanged(string.Empty);
    }
}
