using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace AvaloniaToDoListTrackerAndVisualizer.Models;

public readonly record struct SessionPart
{
    [JsonConstructor]
    public SessionPart(DateTimeOffset partStart, DateTimeOffset partEnd)
    {
        PartStart = partStart;
        PartEnd = partEnd;
        if (PartEnd < partStart)
        {
            throw  new ArgumentOutOfRangeException(nameof(partEnd), "End time must be greater or equal to start time.");
        }
    }

    public DateTimeOffset PartStart { get; }
    public DateTimeOffset PartEnd { get; }
    
    [JsonIgnore]
    public TimeSpan Duration => PartEnd - PartStart;
}

/// <summary>
/// Session consists of many parts - session parts. Each has its start and end point.
/// You can start one session part by calling Start(), which gives you back running session part.
/// You can then End() it (it is added to session, also returns the session part itself), or Cancel().
/// Only one session part can be running at any time, but you can add any custom part with
/// AddPart(SessionPart part). Disposing the running session part automatically ends
/// it correctly, but starting a new one while a different one is in progress cancels the different one.
/// </summary>
public sealed class Session
{
    [JsonIgnore]
    private ObservableCollection<SessionPart> _partsBackingField = new ObservableCollection<SessionPart>();
    
    [JsonInclude]
    [JsonPropertyName(nameof(SessionParts))]
    private ObservableCollection<SessionPart> _parts
    {
        get
        {
            return _partsBackingField;
        }

        set
        {
            _partsBackingField = value;
            SessionParts = new ReadOnlyObservableCollection<SessionPart>(_partsBackingField);
        }
    }
    
    private RunningSessionPart? _runningSessionPart;
    
    private TimeProvider SessionTimeProvider { get; }
    
    /// <summary>
    /// Session will now not try to cancel the old session part
    /// </summary>
    private void NotifyRunningSessionPartStopped()
    {
        _runningSessionPart = null;
    }
    
    [JsonIgnore]
    public ReadOnlyObservableCollection<SessionPart> SessionParts { get; private set; }
    

    public Session():this(TimeProvider.System)
    { }

    /// <summary>
    /// Create new session
    /// </summary>
    /// <param name="timeProvider"> Used to get current UTC time. Injection used for future testing </param>
    public Session(TimeProvider timeProvider)
    {
        SessionParts = new(_parts);
        SessionTimeProvider = timeProvider;
    }

    /// <summary>
    /// Add arbitrary session part (can overlap, but that's not recommended)
    /// </summary>
    public void AddPart(SessionPart part)
    {
        _parts.Add(part);
    }

    /// <summary>
    /// Start new session part (and cancel old one if still running).
    /// Can be used with using().
    /// </summary>
    /// <returns></returns>
    public RunningSessionPart Start()
    {
        if (_runningSessionPart is RunningSessionPart previousPart)
        {
            previousPart.Cancel();
        }
        _runningSessionPart = new RunningSessionPart(SessionTimeProvider.GetUtcNow(), this);
        return _runningSessionPart;
    }

    public TimeSpan TotalSessionTime()
    {
        TimeSpan time = TimeSpan.Zero;
        foreach (var part in SessionParts)
        {
            time += part.Duration;
        }

        return time + (_runningSessionPart?.TimeSoFar ?? TimeSpan.Zero);
    }
    
    public sealed class RunningSessionPart: IDisposable
    {
        private DateTimeOffset PartStart { get; }
        private SessionPart? FinishedSession { get; set; }
        private Session ParentSession { get; }

        public bool Cancelled { get; private set; } = false;

        /// <summary>
        /// Time so far. If cancelled, returns zero, if finished, returns the total time it took.
        /// </summary>
        public TimeSpan TimeSoFar
        {
            get
            {
                if (Cancelled)
                {
                    return TimeSpan.Zero;
                }

                if (Finished)
                {
                    return FinishedSession!.Value.Duration;
                }
                
                return ParentSession.SessionTimeProvider.GetUtcNow() - PartStart;
            }
        }

        /// <summary>
        /// Whether session part successfully finished (not canceled)
        /// </summary>
        public bool Finished
        {
            get
            {
                return FinishedSession is not null;
            }
        }
        

        /// <summary>
        /// If the session part is not cancelled or finished already, finish the task.
        /// That adds it into parent session. If not cancelled, return the finished session part,
        /// otherwise null.
        /// </summary>
        public SessionPart? End()
        {
            if (Finished) return FinishedSession;
            
            if (Cancelled)
            {
                return null;
            }
            
            FinishedSession = new SessionPart(PartStart, ParentSession.SessionTimeProvider.GetUtcNow());
            ParentSession.AddPart(FinishedSession.Value);
            ParentSession.NotifyRunningSessionPartStopped();
            
            return FinishedSession!;
        }

        /// <summary>
        /// Cancels the session part (its time will now be zero), if it didn't finish already.
        /// It now can't be finished.
        /// </summary>
        public void Cancel()
        {
            if (!Finished)
            {
                Cancelled = true;
                ParentSession.NotifyRunningSessionPartStopped();
            }
        }
        
        
        public RunningSessionPart(DateTimeOffset partStart, Session parentSession)
        {
            PartStart = partStart;
            ParentSession = parentSession;
        }
        
        /// <summary>
        /// Correctly ends the running session and counts it in (if it is not already cancelled).
        /// You should use it with using().
        /// </summary>
        public void Dispose()
        {
            End();
        }
    }
}
