using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

/// <summary>
/// This ViewModel is used to show some very basic statistics. // TODO Do super interesting statistics
/// Uses SessionStatistics static class for methods.
/// </summary>
public partial class ProfileViewModel: ViewModelBase, IDisposable
{
        public ProfileViewModel(LocalizationProvider localization, ObservableCollection<Session> sessions, UserSettings userSettings)
        {
                Localization = localization;
                Sessions = sessions;
                UserSettings = userSettings;
                GoalHours = (uint)UserSettings.DailyGoal.Hours;
                GoalMinutes = (uint)UserSettings.DailyGoal.Minutes;

                Localization.PropertyChanged += UpdateLocal;
        }
        
        public LocalizationProvider Localization { get; }
        public UserSettings UserSettings { get; }
        
        /// <summary>
        /// Must be modifiable - there is button to clear them
        /// </summary>
        public ObservableCollection<Session> Sessions { get; }
        
        
        public string TotalTimeText
        {
                get
                {
                        return FormatTimeSpan(SessionStatistics.TotalTimeWorked(Sessions));
                }
        }

        public string MostProductiveDayText
        {
                get
                {
                        return FormatTimeSpan(SessionStatistics.MostProductiveDay(Sessions));
                }
        }

        public string WorkedTodayText
        {
                get
                {
                        return FormatTimeSpan(SessionStatistics.WorkedToday(Sessions));
                }
        }

        public string AveragePerDayText
        {
                get
                {
                        return FormatTimeSpan(SessionStatistics.AveragePerDay(Sessions));
                }
        }

        [ObservableProperty][NotifyPropertyChangedFor(nameof(GoalNotification))] private uint _goalHours;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(GoalNotification))] private uint _goalMinutes;

        // ReSharper disable once UnusedParameterInPartialMethod
        partial void OnGoalHoursChanged(uint value)
        {
                UpdateUserSettings();
        }

        // ReSharper disable once UnusedParameterInPartialMethod
        partial void OnGoalMinutesChanged(uint value)
        {
                UpdateUserSettings();
        }

        /// <summary>
        /// For now, only daily goal is updated
        /// </summary>
        private void UpdateUserSettings()
        {
                UserSettings.DailyGoal = TimeSpan.FromMinutes(GoalMinutes) + TimeSpan.FromHours(GoalHours);
        }

        /// <summary>
        /// Show congratulations text if user achieved goal, or information that they did not yet. 
        /// </summary>
        public string GoalNotification
        {
                get
                {
                        if (TimeSpan.FromMinutes(GoalMinutes) + TimeSpan.FromHours(GoalHours) >= SessionStatistics.WorkedToday(Sessions))
                        {
                                return Localization.GoalNotYetAchievedText;
                        }
                        else
                        {
                                return Localization.GoalAchievedText;
                        }
                }
        }
        
        private string FormatTimeSpan(TimeSpan? timeSpan)
        {
                if (timeSpan is null)
                {
                        // Should not happen
                        return "Unknown";
                }
                else
                {
                        return timeSpan.Value.ToString(@"hh\:mm\:ss");
                }
        }

        /// <summary>
        /// First, show dialog, whether user is sure. If they are, delete all sessions.
        /// Task and groups and everything remains in-tact.
        /// </summary>
        [RelayCommand]
        private async Task DeleteSessionsAsync()
        {
                bool? userIsSure = await WeakReferenceMessenger.Default.Send(new SessionDeletionRequest());
                if (userIsSure is true)
                {
                        Sessions.Clear();
                }
        }

        /// <summary>
        /// Refresh Goal minutes, goal hours and all other properties from the underlying UserSettings model
        /// and sessions. 
        /// </summary>
        [RelayCommand]
        private void RefreshSessions()
        {
                GoalHours = (uint)UserSettings.DailyGoal.Hours;
                GoalMinutes = (uint)UserSettings.DailyGoal.Minutes;
                OnPropertyChanged(String.Empty);
        }
        
        private void UpdateLocal(object? sender, PropertyChangedEventArgs e)
        {
                OnPropertyChanged(nameof(GoalNotification));
        }

        public void Dispose()
        {
                Localization.PropertyChanged -= UpdateLocal;
        }
}
