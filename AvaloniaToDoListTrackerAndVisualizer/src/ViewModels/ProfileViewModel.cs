using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

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

        partial void OnGoalHoursChanged(uint value)
        {
                UpdateUserSettings();
        }

        partial void OnGoalMinutesChanged(uint value)
        {
                UpdateUserSettings();
        }

        private void UpdateUserSettings()
        {
                UserSettings.DailyGoal = TimeSpan.FromMinutes(GoalMinutes) + TimeSpan.FromHours(GoalHours);
        }

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
                        return "Unknown"; //Localization.NotSetText;
                }
                else
                {
                        return timeSpan.Value.ToString(@"hh\:mm\:ss");
                }
        }

        [RelayCommand]
        private async Task DeleteSessionsAsync()
        {
                bool? userIsSure = await WeakReferenceMessenger.Default.Send(new SessionDeletionRequest());
                if (userIsSure is true)
                {
                        Sessions.Clear();
                }
        }

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
