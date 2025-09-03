using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DynamicData;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

/// <summary>
/// The main window view model. The CurrentPage property is what is displayed on the right side.
/// On the left side, there are buttons. This class creates and loads User settings, sessions, groups and tasks via
/// ITaskApplicationFileService. It creates all ViewModels, the corresponding views are then found with ViewLocator in
/// Avalonia.
/// </summary>
public partial class MainWindowViewModel: ViewModelBase
{
    public TaskListViewModel Tasks { get; }
    public UserSettings UserSettings { get; } = new();
    public GroupListViewModel Groups { get; }
    public ObservableCollection<Session> Sessions { get; } = new();
    
    public ITaskApplicationFileService  TaskApplicationFileService { get; }

    private readonly HomeViewModel _home;
    private readonly SettingsViewModel _settings = new SettingsViewModel();
    private readonly ProfileViewModel _profile;
    private readonly TreeViewModel _treeView;

    private static bool FalseConstant { get; } = false;

    public LocalizationProvider Localization { get; } = new();
    
    [ObservableProperty] private ViewModelBase _currentPage;
    
    [RelayCommand]
    private void GoHome () => CurrentPage = _home;
    
    [RelayCommand(CanExecute = nameof(FalseConstant))]
    private void GoSettings () => CurrentPage = _settings;
    
    [RelayCommand]
    private void GoProfile () => CurrentPage = _profile;
    
    [RelayCommand(CanExecute = nameof(FalseConstant))]
    private void GoTreeView () => CurrentPage = _treeView;

    [RelayCommand]
    private void SetLocalization(string cultureInfoString)
    {
        Localization.SetCulture(new CultureInfo(cultureInfoString));
    }

    /// <summary>
    /// Add new task through the TaskEditViewModel dialog.
    /// </summary>
    [RelayCommand]
    private async Task AddNewTask()
    {
        TaskViewModel? result =
            await WeakReferenceMessenger.Default.Send(
                new EditTaskMessage(new TaskViewModel(new TaskModel(Localization.TaskDefaultName), Groups), true, Tasks)
                );
        if (result is not null)
        {
            Tasks.AllTasks.Collection.Add(result);
        }
    }

    public MainWindowViewModel(ITaskApplicationFileService taskApplicationFileService)
    {
        Tasks = new TaskListViewModel(Localization);
        Groups = new GroupListViewModel(Localization);
        _home = new HomeViewModel(Tasks, Groups, Localization);
        _treeView = new TreeViewModel(Localization);
        _profile = new ProfileViewModel(Localization, Sessions, UserSettings);
        CurrentPage = _home;
        
        TaskApplicationFileService = taskApplicationFileService;
        
    }
    
    /// <summary>
    /// Load everything from files. Should be invoked after construction (in Avalonia, from App.axaml.cs).
    /// Warning - data in runtime will be replaced!!!
    /// </summary>
    public async Task LoadFiles()
    {
        try
        {
            Groups.AllGroups.Collection.Clear();
            Tasks.AllTasks.Collection.Clear();
            Sessions.Clear();
            UserSettings.DailyGoal = TimeSpan.Zero;
            
            var taskApplicationStateFromFile = await TaskApplicationFileService.LoadAsync();
            if (taskApplicationStateFromFile is TaskApplicationState taskApplicationState)
            {
                // First add groups so tasks can find them (verify that they have actual group selected)
                Groups.AllGroups.Collection.AddRange(taskApplicationState.Groups);
                // Tasks
                Tasks.AllTasks.Collection.AddRange(
                    taskApplicationState.Tasks.Select(item => new TaskViewModel(item, Groups)));
                // Sessions
                Sessions.AddRange(taskApplicationState.Sessions);
                // User settings
                UserSettings.DailyGoal = taskApplicationState.UserSettings.DailyGoal;
                _profile.RefreshSessionsCommand.Execute(null);
            }
        }
        catch (JsonException)
        {
            // TODO Show error window. Now, the files are just ignored and will be overwritten next time
        }
    }

    /// <summary>
    /// Save all files. Can be done multiple times if you want to save files continuously.
    /// Mainly should be called on main window closed.
    /// </summary>
    public async Task SaveFiles()
    {
        await TaskApplicationFileService.SaveAsync(new TaskApplicationState(Tasks.AllTasks.Collection.Select(tvm => tvm.TaskModel), Groups.AllGroups.Collection, Sessions, UserSettings));
    }

    /// <summary>
    /// For future events - not yet implemented
    /// </summary>
    [RelayCommand(CanExecute = nameof(FalseConstant))]
    private void AddNewEvent()
    {
        
    }

    /// <summary>
    /// Open the session window and let the user do work
    /// </summary>
    [RelayCommand]
    private void StartSession()
    {
        WeakReferenceMessenger.Default.Send(new StartSessionMessage(Tasks, Groups));
    }
}
