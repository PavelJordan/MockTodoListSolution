# Todo list Models

## TaskModels

The main models are the TaskModels - more specifically 
the TaskBaseModel, and the derived classes TaskModel and SubTaskModel.
All are defined in the [TaskModels.cs](../../src/Models/Items/TaskModels.cs) file.
They mainly hold the data of the task using properties that are observable (to know which,
check out the file). The most complicated part is keeping the properties in-sync - in 
other words, detect the changes
in prerequisites, etc. That is also the only responsibility of these classes.
Obviously, TaskModel holds SubTaskModels in ObservableCollection.

The reason that everything is observable is, because some (or rather most) of the data
can be utilized by views directly. Instead of creating the same properties in
another class (viewmodel) and writing the event handling manually,
we can easily just keep the reference to the model public inside the
viewmodel and let View access it directly.
Of course, we still need the viewModel for the UI logic,
but at least it's smaller now.

It also has `Guid Id`, for persistence purposes:

### SaveAbleTaskModels

Because each Task holds references to groups, prerequisites, in the future events,
and also because the properties are observable, it is not serializable. We
need an object that represents it that is serializable.
The answer is [SaveAble task models](../../src/Models/Items/SaveAbleTaskModels.cs).
They also have helper functions to convert between SaveAble and regular versions,
but beware, that you still need to do some things manually - assigning groups
based on ids, the same with prerequisites. For this, the
[task application state](../../src/Models/TaskApplicationState.cs) has more helper
functions, which can do this work for you.

## Group & SaveAbleGroup

Very similarly to how TaskModels are designed are groups. They, too, have
observable properties for the same reason, just like id.

Their file is [here](../../src/Models/Items/Group.cs), and the
SaveAble version is [here](../../src/Models/Items/SaveAbleGroup.cs)

Groups have a name and color. Each TaskModel can have either 0 or 1 group assigned.

## Session

A very important model is [Session](../../src/Models/SessionManagement/Session.cs).
It encapsulates one work session of user - maybe an evening study session. Of course,
during a session, you can have breaks, and so it is broken up into `Session parts`.
These are continuous work session parts and contain these properties:

```C#
    public DateTimeOffset PartStart { get; }
    public DateTimeOffset PartEnd { get; }
    public TimeSpan Duration => PartEnd - PartStart;
```

while duration of the whole session can be computed like this:

```C#
    public TimeSpan TotalSessionTime()
    {
        TimeSpan time = TimeSpan.Zero;
        foreach (var part in SessionParts)
        {
            time += part.Duration;
        }

        return time + (_runningSessionPart?.TimeSoFar ?? TimeSpan.Zero);
    }
```

the latter method also uses `RunningSessionPart` - you receive this object by
calling `Start(TaskViewModel)` and you can call `End()` or `Cancel()` on it.
If you don't Cancel it and only call End, you will receive the finished session part.
It also automatically saves itself into the session, where it was started. Of course,
not always one is running, that's why it's nullable.

There are some invariants to it. For those, I recommend you go through the
[Session](../../src/Models/SessionManagement/Session.cs) file.

Also note that it uses `DateTimeOffset`, whereas TaskModel uses `DateTime`. That is,
because the per-day statistics, and length of sessions part, is strictly measured in UTC.
That way, even if you switch your time, it will still make sense.

On the other hand, TaskModel has your local time in it - because its not measuring anything, you only
use it to visualize, whether deadline is around the corner.

The statistics is computed in the [SessionStatistics](../../src/Models/SessionManagement/SessionStatistics.cs) class.
It is a static class, it only needs IEnumerable of sessions. It also uses UTC.

You can now either go through the code to better understand everything, or, as recommended, you can go through
[ViewModels](ViewModelsDocs.md).
