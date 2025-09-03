# ViewModels

There are three very important view models: MainWindowViewModel,
SessionViewModel and TimerViewModel. Let's go over all of these and explain their purposes.

## MainWindowViewModel

[MainWindowViewModel](../../src/ViewModels/MainWindowViewModel.cs) is the most important class here.
It holds global collections of tasks, groups, sessions, usersettings and LocalizationProvider.
It also has reference to other ViewModels - HomeViewModel, ProfileViewModel and can create more
ViewModels as needed - EditTaskViewModel, SessionViewModel, etc.

In the view, it has buttons on the left side and then content panel on the right side. This panel
is determined by `CurrentPage` property, which can have any ViewModel assigned. The Avalonia
`ViewLocator` then finds the correct View.

The possible contents of the right side are now:

 - `HomeViewModel`
 - `ProfileViewModel`
 - `SettingsViewModel` (not implemented)
 - `TreeViewModel` (not implemented)

It also has `LoadFiles()` and `SaveFiles()` methods, which load/save data for persistence.

Because buttons open dialogs or other windows, the view must be notified to do so. For this,
the `MainWindowViewModel` uses these messages:

 - EditTaskMessage
 - OpenGroupSelectionRequest
 - StartSessionMessage
 - SessionDeletionRequest

these are sent either by MainWindowViewModel itself, or by ViewModel inside its content window.
Other ViewModels work similarly with messages, using `WeakReferenceMessenger.Default`.
For more information, check out the [MainWindowViewModel](../../src/ViewModels/MainWindowViewModel.cs)
file together with its associated [view](../../src/Views/MainWindow.axaml.cs).

## SessionViewModel

The [SessionViewModel](../../src/ViewModels/SessionViewModels/SessionViewModel.cs)
again has many other ViewModels visible at once - mainly the selected/not selected
task information on the right, and timer on the left. Through the buttons on the right,
the SessionViewModel sends start/stop commands to the timer on the left.

The Session model actually lives inside the TimerViewModel, because that is
where it's required - to help timer count the worked time.

Again, in order to understand the class fully, read it in code:
[SessionViewModel](../../src/ViewModels/SessionViewModels/SessionViewModel.cs).

## TimerViewModel

[TimerViewModel](../../src/ViewModels/SessionViewModels/Timer/TimerViewModel.cs) contains
two sessions - one for working, on for breaks (for pomodoro specifically). It also has
`SelectedTimer` property, which is either `PomodoroTimerViewModel` or `RegularTimerViewModel`, depending on
which timer is selected in TimerType property (anyone can change it). It then remembers,
which task is selected (previewed), and once the timer starts, it goes to TaskToWorkOn. Time on
screen is updated with `DispatcherTimer _refreshTimer`, which is set up to raise
PropertyChanged event every second (while timer is running), which causes Views to refresh.

Its main difficulty is keeping valid state (working/not working? Pomodoro? Regular) and adding up
the worked time correctly to tasks or pomodoro timer view model. Look into the file to know more.

## TaskViewModel, TaskListViewModel

The last two view models I want to talk about are
[TaskViewModel](../../src/ViewModels/ItemViewModels/TaskViewModel.cs) and
[TaskListViewModel](../../src/ViewModels/ItemViewModels/TaskListViewModel.cs).
TaskViewModel wraps around TaskModel, exposes it, and also gives out many more properties and
commands for UI use.  The biggest responsibility of TaskViewModel, however,
is to keep all properties for graphical representation
in sync. For that reason, there are many methods,
like `void UpdateViewModelProperties(object? sender, PropertyChangedEventArgs e)`.

There is also `SubTaskViewModel`, and because taskModel holds only Models of subtasks, not
ViewModels, TaskViewModel creates them with `DynamicData library`:

```C#
_subTaskViewModelsPipeline = taskModel.Subtasks
            .ToObservableChangeSet()
            .Transform(subTaskModel => new SubTaskViewModel(subTaskModel, Localization))
            .Bind(out _subTasksViewModels)
            .DisposeMany()
            .Subscribe();
```

`TaskListViewModel`, on the other hand, contains mainly
`public ObservableChildrenCollectionWrapper<TaskViewModel> AllTasks { get; }`.
(Here we are using the wrapper mentioned in [README.md](README.md)). It also has many
ReadOnly variants, like `ReadyTasks` or `CompletedTasks`. TaskListViewModel needs to keep all of these
in sync, which is achieved thanks to the `ObservableChildrenCollectionWrapper`.

Again, look into the code to have the whole picture.