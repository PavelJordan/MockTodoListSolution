using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace AvaloniaToDoListTrackerAndVisualizer.Models;

/// <summary>
/// One part of a session, with start and end time without any breaks.
/// Uses UTC time in case user decides to cross the borders.
/// </summary>
public readonly record struct SessionPart
{
    private const string END_TIME_MUST_BE_GREATER_OR_EQUAL_TO_START_TIME = "End time must be greater or equal to start time.";
    
    /// <exception cref="ArgumentOutOfRangeException"> if end date is earlier than start date </exception>
    [JsonConstructor]
    public SessionPart(DateTimeOffset partStart, DateTimeOffset partEnd)
    {
        PartStart = partStart;
        PartEnd = partEnd;
        if (PartEnd < partStart)
        {
            
            throw  new ArgumentOutOfRangeException(nameof(partEnd), END_TIME_MUST_BE_GREATER_OR_EQUAL_TO_START_TIME);
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
/// Uses UTC time in case user decides to cross the borders.
/// </summary>
public sealed class Session
{
    [JsonIgnore]
    private ObservableCollection<SessionPart> _partsBackingField = new ObservableCollection<SessionPart>();
    
    /// <summary>
    /// When Json loads this up, it assigns new ObservableCollection - so it needs to be refreshed,
    /// hence the custom setter. The public readonly for this is SessionParts.
    /// </summary>
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
    
    /// <summary>
    /// Part that is running - to cancel it if user decides to start new one or read time.
    /// Must be null if no part is running!!!
    /// </summary>
    private RunningSessionPart? _runningSessionPart;
    
    private TimeProvider SessionTimeProvider { get; }
    
    /// <summary>
    /// Session will now not try to cancel the old session part
    /// </summary>
    private void NotifyRunningSessionPartStopped()
    {
        _runningSessionPart = null;
    }
    
    /// <summary>
    /// All parts that are finished so far. Do not need to be in order
    /// </summary>
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
    /// Add arbitrary session part (can overlap, but that is not recommended)
    /// </summary>
    public void AddPart(SessionPart part)
    {
        _parts.Add(part);
    }

    /// <summary>
    /// Start new session part (and cancel old one if still running).
    /// Can be used with using().
    /// Also returns the RunningSessionPart, where you can see,
    /// how long it is running. It is also used to stop or cancel the session,
    /// which then automatically adds it to this session as new part session.
    /// </summary>
    public RunningSessionPart Start()
    {
        if (_runningSessionPart is RunningSessionPart previousPart)
        {
            previousPart.Cancel();
        }
        _runningSessionPart = new RunningSessionPart(SessionTimeProvider.GetUtcNow(), this);
        return _runningSessionPart;
    }

    /// <summary>
    /// Counts total session time (finished and plus running if it exists)
    /// </summary>
    public TimeSpan TotalSessionTime()
    {
        TimeSpan time = TimeSpan.Zero;
        foreach (var part in SessionParts)
        {
            time += part.Duration;
        }

        return time + (_runningSessionPart?.TimeSoFar ?? TimeSpan.Zero);
    }
    
    /// <summary>
    /// Class that holds currently running session part with known Session parent.
    /// It can be ended (which then assigns the finished session part into Session),
    /// canceled, and you can read time that it is running.
    /// Uses UTC time in case user decides to cross the borders.
    /// </summary>
    public sealed class RunningSessionPart: IDisposable
    {
        private DateTimeOffset PartStart { get; }
        
        /// <summary>
        /// If this session ended, the Finished session is saved here for retrieval of total duration.
        /// </summary>
        private SessionPart? FinishedSession { get; set; }
        
        /// <summary>
        /// Where to save the finished session
        /// </summary>
        private Session ParentSession { get; }

        /// <summary>
        /// Task can be cancelled only if it is not finished already. After this,
        /// it cannot be finished anymore
        /// </summary>
        public bool Cancelled { get; private set; } = false;

        /// <summary>
        /// Time so far. If cancelled, returns zero, if finished, returns the total time it took.
        /// Otherwise, returns the current time so far in progress.
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
        
        /// <summary>
        /// Create the running session part manually. Can't recommend.
        /// </summary>
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
