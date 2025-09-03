using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaToDoListTrackerAndVisualizer.Models;

/// <summary>
/// Provides static methods to analyze enumerable sessions and take useful renderable data from them
/// </summary>
public static class SessionStatistics
{ 
        public static TimeSpan TotalTimeWorked(IEnumerable<Session> sessions)
        {
                TimeSpan ts = TimeSpan.Zero;
                foreach (Session session in sessions)
                {
                        ts += session.TotalSessionTime();
                }

                return ts;
        }
        
        /// <summary>
        /// Uses UTC time, if session parts go over multiple days, use the latter.
        /// </summary>
        public static TimeSpan WorkedToday(IEnumerable<Session> sessions)
        {
                TimeSpan ts = TimeSpan.Zero;
                foreach (Session session in sessions)
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

        /// <summary>
        /// Uses UTC time, if session parts go over multiple days, use the latter.
        /// </summary>
        public static TimeSpan MostProductiveDay(IEnumerable<Session> sessions)
        {
                // record Date - time so far
                Dictionary<DateOnly, TimeSpan> dayTimes = new();
                
                foreach (Session session in sessions)
                {
                        foreach (SessionPart part in session.SessionParts)
                        {
                                if (!dayTimes.TryAdd(DateOnly.FromDateTime(part.PartEnd.UtcDateTime.Date), part.Duration))
                                {
                                        dayTimes[DateOnly.FromDateTime(part.PartEnd.UtcDateTime.Date)] += part.Duration;
                                }
                        }
                }

                // Edge case
                if (dayTimes.Count == 0)
                {
                        return TimeSpan.Zero;
                }
                
                return dayTimes.Values.Max();
        }
        
        /// <summary>
        /// Uses UTC time, if session parts go over multiple days, use the latter.
        /// </summary>
        public static TimeSpan AveragePerDay(IEnumerable<Session> sessions)
        {
                // record Date - time so far
                Dictionary<DateOnly, TimeSpan> dayTimes = new();
                
                foreach (Session session in sessions)
                {
                        foreach (SessionPart part in session.SessionParts)
                        {
                                if (!dayTimes.TryAdd(DateOnly.FromDateTime(part.PartEnd.UtcDateTime.Date), part.Duration))
                                {
                                        dayTimes[DateOnly.FromDateTime(part.PartEnd.UtcDateTime.Date)] += part.Duration;
                                }
                        }
                }

                // Edge case
                if (dayTimes.Count == 0)
                {
                        return TimeSpan.Zero;
                }
                
                return TimeSpan.FromSeconds(dayTimes.Values.Sum(x => x.TotalSeconds) /  dayTimes.Count);
        }
}
