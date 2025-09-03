# Views and UX Components

## Main window

 [MainWindow.axaml](../../src/Views/MainWindow.axaml) hosts the entire application. It uses`CurrentPage`property of its view model to
switch between pages on the right (Home, Settings (not implemented), Tree (not implemented), Profile)
as commanded by the navigation buttons. The navigation side of the window also has buttons to create new tasks or
start session. When the "start session" button is clicked the view model publishes a
[StartSessionMessage](../../src/Messages/StartSessionMessage.cs) which results in a [SessionWindow](../../src/Views/SessionViews/SessionWindow.axaml) being opened by this view.

## Home view

[HomeView.axaml](../../src/Views/HomeView.axaml)  presents list of tasks. It binds to [TaskListViewModel](../../src/ViewModels/ItemViewModels/TaskListViewModel.cs) in its ViewModel to
display all the tasks. The view contains three toggle buttons ("Ready", "Done" and "All")
to switch between filters from TaskListViewModel.
The list view uses `DataTemplate` to display each [TaskViewModel](../../src/ViewModels/ItemViewModels/TaskViewModel.cs)s information, group name with color and action
button for each. When the user chooses to edit a task, the view model publishes a message and the
view receives it, opening the dialog. If view model publishes TaskDeletion message, that one is received by
TaskListViewModel.

## Task edit view

Task editing is in [TaskEditView](../../src/Views/ItemEditViews/TaskEditView.axaml). The window's ViewModel exposes the TaskViewModel with its properties.
There is button for managing prerequisites - clicking it sends a messages that
open [PrerequisiteTaskSelectionDialog](../../src/Views/SelectionViews/PrerequisiteTaskSelectionDialog.axaml). There is also button for adding subtasks - that one simply adds it,
but that exposes new button - to edit the subtask. Each subtask has this button, and again, message is
sent by clicking on it, and message receiver opens the dialog. It subscribes to
[CloseEditMessage](../../src/Messages/CloseMessages/CloseEditMessage.cs) to go back (or save in case the task is new) and close the window.

## Profile view

[ProfileView.axaml](../../src/Views/ProfileView.axaml) displays statistics about completed sessions. It binds to its [ProfileViewModel](../../src/ViewModels/ProfileViewModel.cs),
which exposes properties like total time worked, longest work day and user goal.
When a session deletion is requested it invokes [SessionDeletionRequest](../../src/Messages/DeleteRequests/SessionDeletionRequest.cs).

## Session window and timer

After clicking the start session button, this window opens up.
On the left side, there is a timer, and on the right side, either
button to select task (then it is [NotSelectedTaskSessionView](../../src/Views/SessionViews/NotSelectedTaskSessionView.axaml)),
or selected task with buttons (then it is [SelectedTaskSessionView](../../src/Views/SessionViews/SelectedTaskSessionView.axaml)).
This switch is achieved by switching corresponding ViewModel field in [SessionViewModel](../../src/ViewModels/SessionViewModels/SessionViewModel.cs), so
Avalonia can find the corresponding `View` with `ViewLocator`.

The timer on the left works similarly, with two possible
Views - [RegularTimerView](../../src/Views/SessionViews/Timer/RegularTimerView.axaml) and
[PomodoroTimerView](../../src/Views/SessionViews/Timer/RegularTimerView.axaml). Again, `ViewLocator` does its job here. 

The session window listens to these messages:

 - [SessionTaskSelectionRequest](../../src/Messages/TaskSelectionRequest.cs) -> open dialog and expect [TaskViewModel](../../src/ViewModels/ItemViewModels/TaskViewModel.cs). Then respond with it
 - [EditTaskInSessionMessage](../../src/Messages/EditMessages/EditTaskInSessionMessage.cs) -> open EditView. Only give it `SelectedTaskModel`, so it can change it how it wants.
 - [SetupTimerRequest](../../src/Messages/SetupTimerRequest.cs) -> Open TimerSelectionDialog. Only give it [TimerViewModel](../../src/ViewModels/SessionViewModels/Timer/TimerViewModel.cs), so it can modify it how it wants.

## Conclusion

What now? If you read all the documentation, I now recommend that you go through the code. You should now
understand most of the things - it's basically rinse and repeat. Good luck!
