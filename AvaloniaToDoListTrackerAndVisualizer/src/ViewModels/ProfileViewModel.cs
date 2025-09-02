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

        public TimeSpan TotalTimeWorked
        {
                get
                {
                        TimeSpan ts = TimeSpan.Zero;
                        foreach (Session session in Sessions)
                        {
                                ts += session.TotalSessionTime();
                        }

                        return ts;
                }
        }
        
        public TimeSpan WorkedToday
        {
                get
                {
                        TimeSpan ts = TimeSpan.Zero;
                        foreach (Session session in Sessions)
                        {
                                foreach (SessionPart part in session.SessionParts)
                                {
                                        if (part.PartEnd.UtcDateTime.Date == DateTimeOffset.UtcNow.Date)
                                        {
                                                ts += part.Duration;
                                        }
                                }
                        }

                        return ts;
                }
        }

        public TimeSpan MostProductiveDay
        {
                get
                {
                        Dictionary<DateOnly, TimeSpan> dayTimes = new();
                        foreach (Session session in Sessions)
                        {
                                foreach (SessionPart part in session.SessionParts)
                                {
                                        if (!dayTimes.TryAdd(DateOnly.FromDateTime(part.PartEnd.UtcDateTime.Date), part.Duration))
                                        {
                                                dayTimes[DateOnly.FromDateTime(part.PartEnd.UtcDateTime.Date)] += part.Duration;
                                        }
                                }
                        }

                        if (dayTimes.Count == 0)
                        {
                                return TimeSpan.Zero;
                        }
                        
                        return dayTimes.Values.Max();
                }
        }
        
        public TimeSpan AveragePerDay
        {
                get
                {
                        Dictionary<DateOnly, TimeSpan> dayTimes = new();
                        
                        foreach (Session session in Sessions)
                        {
                                foreach (SessionPart part in session.SessionParts)
                                {
                                        if (!dayTimes.TryAdd(DateOnly.FromDateTime(part.PartEnd.UtcDateTime.Date), part.Duration))
                                        {
                                                dayTimes[DateOnly.FromDateTime(part.PartEnd.UtcDateTime.Date)] += part.Duration;
                                        }
                                }
                        }

                        if (dayTimes.Count == 0)
                        {
                                return TimeSpan.Zero;
                        }
                        
                        return TimeSpan.FromSeconds(dayTimes.Values.Sum(x => x.TotalSeconds) /  dayTimes.Count);
                }
        }
        
        public string TotalTimeText
        {
                get
                {
                        return FormatTimeSpan(TotalTimeWorked);
                }
        }

        public string MostProductiveDayText
        {
                get
                {
                        return FormatTimeSpan(MostProductiveDay);
                }
        }

        public string WorkedTodayText
        {
                get
                {
                        return FormatTimeSpan(WorkedToday);
                }
        }

        public string AveragePerDayText
        {
                get
                {
                        return FormatTimeSpan(AveragePerDay);
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
                        if (TimeSpan.FromMinutes(GoalMinutes) + TimeSpan.FromHours(GoalHours) >= WorkedToday)
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
