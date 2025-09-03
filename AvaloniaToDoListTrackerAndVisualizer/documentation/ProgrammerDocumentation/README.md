# Programmer documentation

Welcome to the programmer documentation! Here, you will find architectural overview of the application,
with some brief explanation of some important classes.

Note: this documentation is short for now. You can checkout
[AI generated programmer documentation](../AIGeneratedProgrammerDocumentationFallback.md), which is a little
longer and more complete, but lacks the understanding and ideas I have about this application. For this reason,
I recommend you first read my programmer documentation, and after that, you can go over the AI generated one.
After that, feel free to browse the code, as I populated it with many helpful documentation comments.

## Before you start

I highly recommend you first [learn some basics about avalonia](https://docs.avaloniaui.net/docs/get-started/). At least
get to building the sample to-do list app. After that, you are more or less ready to browse this documentation and code,
but you will probably still have to go check some avalonia tutorials for better understanding. Some C# knowledge
is a must before contribution.

## Project structure

    TodoListSolution/
    ├── AvaloniaToDoListTrackerAndVisualizer/
    │   ├── Assets/                # icons
    │   ├── Lang/                  # resource files for localization (en‑US, cs‑CZ)
    │   ├── documentation/         # documentation
    │   ├── src/
    │   │   ├── Converters/        # value converters for Avalonia bindings
    │   │   ├── Extensions/        # helper extension methods
    │   │   ├── Messages/          # weak‑reference messages for MVVM communication
    │   │   ├── Models/            # core data models and save‑able types
    │   │   ├── Providers/         # localization provider
    │   │   ├── Services/          # file persistence service
    │   │   ├── Styles/            # Axaml style definitions
    │   │   ├── ViewModels/        # all view models, grouped by purpose
    │   │   ├── Views/             # Axaml files and code‑behind for UI
    │   │   └── Wrappers/          # collection wrapper tracking child changes
    │   ├── App.axaml              # application XAML and resources
    │   ├── App.axaml.cs           # application start‑up logic
    │   ├── Program.cs             # entry point
    │   └── ViewLocator.cs         # resolves view models to views
    └── TodoListSolution.sln       # solution file

## High-level architectural overview

As the application follows Avalonia MVVM guidelines, we have 3 main parts:

 - **Models** - contain the data and business logic (like validation, useful methods and encapsulation). They already have
    observable properties so the ViewModels do not need to write additional boilerplate, but it is decoupled as much as possible.
    In fact, they never reference Views or ViewModels! You can find TaskModel and SubTaskModel here, or even Session class and SessionStatistics.

 - **ViewModels** - in order to have UI that can manipulate with these models, we need a middle man - who will have commands the user
    can call, or text that should be shown, or colors which should be applied to the background. Who will manage, which
    tasks should be visible, which window should be visible, etc. As much as possible from the UI and graphics logic is here.
    some important ViewModels are for example HomeViewModel, which contains TaskListViewModel. Together, they give you
    ObservableCollections, thanks to which you can see all the tasks on the screen or filter them.
    Also, SessionViewModel is important - it contains the timer with Session object, and decides, which timer should be
    visible when. To be able to distinguish, what should be a ViewModel, think about it like this: you probably need
    to save models to persistent storage. ViewModels (with window state) not.

 - **Views** - those are the things user sees. It's just a bunch of buttons and textboxes, maybe with a layout. And these
    are bound to the ViewModel properties, which is injected upon creation. View is minimal from this point of view -
    you only set that the button should call a Command on viewmodel. Nothing else. But sometimes, you need view to do something.
    Maybe show a dialog. For this, messages are used - the application uses WeakReferenceMessenger so ViewModels can signal
    Views the need to do some action, like open a dialog and retrieve new task. ViewModels, however, never reference
    views directly.

Sometimes though, we need to be able to have a View inside ViewModel (for example, MainWindowViewModel needs to be able to
display Home view, Profile view...). For this reason, avalonia `ViewLocator` is used. This way, even though, you have this:

```C#
private readonly HomeViewModel _home;
private readonly SettingsViewModel _settings = new SettingsViewModel();
private readonly ProfileViewModel _profile;
private readonly TreeViewModel _treeView;
```

when your view binds to these values, the corresponding view is created instead, with this DataContext (ViewModels) injected!
This is heavily used in this application, together with messages, to achieve zero coupling of viewModels to views.

## What happens after startup

[Program.cs](../../src/Program.cs) starts, builds the avalonia application, and `OnFrameworkInitializationCompleted()`
is called inside [App.axaml.cs](../../src/App.axaml.cs). This then initializes our [MainWindow](../../src/Views/MainWindow.axaml.cs)
with it's DataContext injected as [MainWindowViewModel](../../src/ViewModels/MainWindowViewModel.cs).

[App.axaml.cs](../../src/App.axaml.cs) also calls FileLoading methods on [MainWindowViewModel](../../src/ViewModels/MainWindowViewModel.cs)
so we have data loaded (and on main window closing it also saves the data). Then, main window is open. You can check out
[MainWindowViewModel](../../src/ViewModels/MainWindowViewModel.cs) constructor to know more about what is happening.

## Localization

Localization is managed via [LocalizationProvider.cs](../../src/Providers/LocalizationProvider.cs) and the translations
are held at [Resources.resx](../../Lang/Resources.resx). 

The problem with using solely [Resources.resx](../../Lang/Resources.resx) was that when languages were switched during
runtime, the Views were not notified. Because restarting application sounded very inconvenient,
[LocalizationProvider.cs](../../src/Providers/LocalizationProvider.cs) was added. It wraps each property from
[Resources.resx](../../Lang/Resources.resx) into its own property and also exposes method `void SetCulture(CultureInfo culture)`,
which raises `PropertyChanged` event for all properties. This way, all views are notified and updated at run-time.

This causes a little boilerplate. If you know how to make the property generation
in [LocalizationProvider.cs](../../src/Providers/LocalizationProvider.cs) automatic, please, do so!

Please note that there should be only one instance of [LocalizationProvider.cs](../../src/Providers/LocalizationProvider.cs)
at any time.

## Persistence

For the purpose of persistence, we have [this structure](../../src/Models/TaskApplicationState.cs):

```C#
public record struct TaskApplicationState
{
    public IEnumerable<TaskModel> Tasks { get; }
    public IEnumerable<Group> Groups { get; }
    public IEnumerable<Session> Sessions { get; }
    public UserSettings UserSettings { get; }
    // and more methods...
```

and [this interface](../../src/Services/ITaskApplicationFileService.cs):

```C#
public interface ITaskApplicationFileService
{
    public Task<TaskApplicationState?> LoadAsync();
    public Task SaveAsync(TaskApplicationState applicationStateToSave);
}
```

as you can see, TaskApplicationState can be used both to save the current state, and to retrieve it.
The exact data that you save go out when loaded. One limitation is that you can read from `TaskApplicationState`
only once.

The proper implementation of the file service is in the same file, while the object is used from the
[MainWindowViewModel](../../src/ViewModels/MainWindowViewModel.cs). To inject the service, this application
uses DependencyInjection:

```C#
// In main window initialization
        var collection = new ServiceCollection();
        collection.AddCommonServices();
        
        var services = collection.BuildServiceProvider();
        
        _mainWindowViewModel = services.GetService<MainWindowViewModel>()!;

// Further below
public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<ITaskApplicationFileService, TaskApplicationFileService>();
        collection.AddTransient<MainWindowViewModel>();
    }
}
```

The code is from [this file](../../src/App.axaml.cs). You can learn more about it
[here](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection).

For now, there is no way to restore corrupted files. Right now, they are just overwritten. If you want, you can
contribute!

## Collection wrapper

Because there are a lot of collections, many times, the application required to subscribe to
events of all the elements of those collections. This would get tedious very fast. For this reason,
we have this class:

[ObservableChildrenCollectionWrapper<T>](../../src/Wrappers/ObservableChildrenCollectionWrapper.cs),
which wraps on `ObservableCollection<T>`.

It exposes `ChildrenPropertyChanged event` so users can listen for property changes on its items very easily.
It rewires subscriptions when items are added, removed or replaced.

## Next sections

Next, depending on what you want to learn, you can check out the following files. They teach you more in detail about
the specific subjects and how it works in this application:

 - [TodoList Models](ModelsDocs.md)
 - [TodoList ViewModels](ViewModelsDocs.md)
 - [TodoListViews, messages and converters](ViewsDocs.md)
