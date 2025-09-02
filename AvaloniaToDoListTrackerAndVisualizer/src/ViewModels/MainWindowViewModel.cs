using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DynamicData;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

/// <summary>
/// The main window view model. The CurrentPage property is what is displayed.
/// </summary>
public partial class MainWindowViewModel: ViewModelBase
{
    public TaskListViewModel Tasks { get; } =  new();
    public GroupListViewModel Groups { get; }
    public List<Session> Sessions { get; } = new();
    
    public ITaskApplicationFileService  TaskApplicationFileService { get; }

    private readonly ViewModelBase _home;
    private readonly ViewModelBase _settings = new SettingsViewModel();
    private readonly ViewModelBase _profile;
    private readonly ViewModelBase _treeView;

    private static bool FalseConstant { get; } = false;

    public LocalizationProvider Localization { get; } = new();
    
    [ObservableProperty] private ViewModelBase _currentPage;
    
    [RelayCommand]
    private void GoHome () => CurrentPage = _home;
    
    [RelayCommand(CanExecute = nameof(FalseConstant))]
    private void GoSettings () => CurrentPage = _settings;
    
    [RelayCommand(CanExecute = nameof(FalseConstant))]
    private void GoProfile () => CurrentPage = _profile;
    
    [RelayCommand(CanExecute = nameof(FalseConstant))]
    private void GoTreeView () => CurrentPage = _treeView;

    [RelayCommand]
    private void SetLocalization(string cultureInfoString)
    {
        Localization.SetCulture(new CultureInfo(cultureInfoString));
    }

    [RelayCommand]
    private async Task AddNewTask()
    {
        TaskViewModel? result =
            await WeakReferenceMessenger.Default.Send(
                new EditTaskMessage(new TaskViewModel(new TaskModel(Localization.TaskDefaultName), Groups, Localization), true, Tasks)
                );
        if (result is not null)
        {
            Tasks.AllTasks.Collection.Add(result);
        }
    }

    public MainWindowViewModel(ITaskApplicationFileService taskApplicationFileService)
    {
        Groups = new GroupListViewModel(Localization);
        _home = new HomeViewModel(Tasks, Groups, Localization);
        _treeView = new TreeViewModel(Localization);
        _profile = new ProfileViewModel(Localization);
        CurrentPage = _home;
        
        TaskApplicationFileService = taskApplicationFileService;
        
    }
    
    public async Task LoadFiles()
    {
        var taskApplicationStateFromFile = await TaskApplicationFileService.LoadFromFileAsync();
        if (taskApplicationStateFromFile is TaskApplicationState taskApplicationState)
        {
            // First add groups so tasks can find them (verify that they have actual group selected)
            Groups.AllGroups.Collection.AddRange(taskApplicationState.Groups);
            Tasks.AllTasks.Collection.AddRange(taskApplicationState.Tasks.Select(item => new TaskViewModel(item, Groups, Localization)));
            Sessions.AddRange(taskApplicationState.Sessions);
        }
    }

    public async Task SaveFiles()
    {
        await TaskApplicationFileService.SaveToFileAsync(new TaskApplicationState(Tasks.AllTasks.Collection.Select(tvm => tvm.TaskModel), Groups.AllGroups.Collection, Sessions));
    }

    [RelayCommand(CanExecute = nameof(FalseConstant))]
    private void AddNewEvent()
    {
        
    }

    [RelayCommand]
    private void StartSession()
    {
        WeakReferenceMessenger.Default.Send(new StartSessionMessage(Tasks, Groups));
    }
}
