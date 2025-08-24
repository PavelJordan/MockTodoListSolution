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
    public string DeleteButton => Resources.DeleteButton;
    public string DeleteSwitchOff => Resources.DeleteSwitchOff;
    public string DeleteSwitchOn => Resources.DeleteSwitchOn;
    public string DoneButton => Resources.DoneButton;
    public string HomeButton => Resources.HomeButton;
    public string ProfileButton => Resources.ProfileButton;
    public string ReadyButton => Resources.ReadyButton;
    public string SettingsButton => Resources.SettingsButton;
    public string StartSessionButton => Resources.StartSessionButton;
    public string TreeButton => Resources.TreeButton;
    public string UnCompleteButton => Resources.UnCompleteButton;
    
    public string EditTaskWindowName => Resources.EditTaskWindowName;
    
    public string GoBackButton => Resources.GoBackButton;
    public string SaveAndBackButton => Resources.SaveAndBackButton;
    public string TaskDefaultName => Resources.TaskDefaultName;
    public string BeginDateText => Resources.BeginDateText;
    public string SoftDeadlineText => Resources.SoftDeadlineText;
    public string HardDeadlineText => Resources.HardDeadlineText;

    public string DaysAfterBeginDateNoDeadline => Resources.DaysAfterBeginNoDeadline;
    public string DaysAfterHard => Resources.DaysAfterHard;
    public string DaysAfterSoft => Resources.DaysAfterSoft;
    public string DaysUntilBeginDate => Resources.DaysUntilBeginDate;
    public string DaysUntilHardDeadline => Resources.DaysUntilHardDeadline;
    public string DaysUntilSoftDeadline => Resources.DaysUntilSoftDeadline;
    public string NoDeadlineSet => Resources.NoDeadlineSet;

    public void SetCulture(CultureInfo culture)
    {
        Resources.Culture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        OnPropertyChanged(string.Empty);
    }
}
