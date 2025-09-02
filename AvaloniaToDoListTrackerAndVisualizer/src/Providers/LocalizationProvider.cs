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
    public string HoursText => Resources.HoursText;
    public string MinutesText => Resources.MinutesText;
    public string NotSetText => Resources.NotSetText;
    public string TimeExpectedText => Resources.TimeExpectedText;
    public string TimeSpentText => Resources.TimeSpentText;
    
    public string SubTaskEditWindowName => Resources.SubTaskEditWindowName;

    public string AddSubTaskButton => Resources.AddSubTaskButton;
    public string NewSubTaskName => Resources.NewSubTaskName;
    public string DescriptionText => Resources.DescriptionText;
    public string NameText => Resources.NameText;
    
    public string AssignNoGroupButton => Resources.AssignNoGroupButton;
    public string GroupNameWatermark => Resources.GroupNameWatermark;
    public string GroupSelectionWindowName => Resources.GroupSelectionWindowName;
    public string NewGroupButton => Resources.NewGroupButton;
    public string NewGroupPlaceholderName => Resources.NewGroupPlaceholderName;
    public string SelectGroupText => Resources.SelectGroupText;
    public string SelectText => Resources.SelectText;
    public string CancelButton => Resources.CancelButton;
    public string ComingSoonText => Resources.ComingSoonText;
    public string LongBreakTimeText => Resources.LongBreakTimeText;
    public string ManagePrerequisitesText => Resources.ManagePrerequisitesText;
    public string PomodoroTimerText => Resources.PomodoroTimerText;
    public string PrerequisitesSelectionDialog => Resources.PrerequisiteSelectionDialog;
    public string RegularTimerText => Resources.RegularTimerText;
    public string ResetButton => Resources.ResetButton;
    public string SelectedTasksText => Resources.SelectedTasksText;
    public string SelectTasksText => Resources.SelectTasksText;
    public string SelectTaskText => Resources.SelectTaskText;
    public string SelectTaskToWorkOnText => Resources.SelectTaskToWorkOnText;
    public string SessionTaskSelectionDialog => Resources.SessionTaskSelectionDialog;
    public string SessionTotalText => Resources.SessionTotalText;
    public string SessionWindow => Resources.SessionWindow;
    public string ShortBreaksBeforeLongText => Resources.ShortBreaksBeforeLongText;
    public string ShortBreakTimeText => Resources.ShortBreakTimeText;
    public string SubtaskText => Resources.SubtaskText;
    public string TaskText => Resources.TaskText;
    public string TestNotificationSoundText => Resources.TestNotificationSoundText;
    public string TimerSelectionDialog => Resources.TimerSelectionDialog;
    public string ToggleSelectionButton => Resources.ToggleSelectionButton;
    public string WorkTimeText => Resources.WorkTimeText;
    public string RemoveButton => Resources.RemoveButton;
    public string EndText => Resources.EndText;
    public string SkipBreakText => Resources.SkipBreakText;
    public string SkipWorkText => Resources.SkipWorkText;
    public string WorkLeftText => Resources.WorkLeftText;
    public string OvertimeText => Resources.OvertimeText;
    public string BreakLeftText => Resources.BreakLeftText;
    public string OverBreakText => Resources.OverBreakText;
    public string NoGroupText => Resources.NoGroupText;
    public string AveragePerDay => Resources.AveragePerDay;
    public string DeleteSessionsDataText => Resources.DeleteSessionsDataText;
    public string GoalNotYetAchievedText  => Resources.GoalNotYetAchievedText;
    public string GoalText => Resources.GoalText;
    public string MostProductiveDayText => Resources.MostProductiveDayText;
    public string No => Resources.No;
    public string Yes => Resources.Yes;
    public string RefreshButtonText => Resources.RefreshButtonText;
    public string SessionDeletionDialog => Resources.SessionDeletionDialog;
    public string SessionDeletionVerification => Resources.SessionDeletionVerification;
    public string TotalTimeText => Resources.TotalTimeText;
    public string WorkedTodayText =>  Resources.WorkedTodayText;
    public string GoalAchievedText => Resources.GoalAchievedText;

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
