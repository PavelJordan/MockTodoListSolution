using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using AvaloniaToDoListTrackerAndVisualizer.Messages;
using AvaloniaToDoListTrackerAndVisualizer.Models.Items;
using AvaloniaToDoListTrackerAndVisualizer.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

/// <summary>
/// The main window view model. The CurrentPage property is what is displayed.
/// </summary>
public partial class MainWindowViewModel: ViewModelBase
{
    [ObservableProperty]
    private TaskListViewModel _tasks =  new();
    
    public ITaskModelFileService  TaskModelFileService { get; }

    private readonly ViewModelBase _home;
    private readonly ViewModelBase _settings = new SettingsViewModel();
    private readonly ViewModelBase _profile;
    private readonly ViewModelBase _treeView;

    public LocalizationProvider Localization { get; } = new();
    
    [ObservableProperty] private ViewModelBase _currentPage;
    
    [RelayCommand]
    private void GoHome () => CurrentPage = _home;
    
    [RelayCommand]
    private void GoSettings () => CurrentPage = _settings;
    
    [RelayCommand]
    private void GoProfile () => CurrentPage = _profile;
    
    [RelayCommand]
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
                new EditTaskMessage(new TaskViewModel(new TaskModel(Localization.TaskDefaultName), Localization), true)
                );
        if (result is not null)
        {
            Tasks.AllTasks.Collection.Add(result);
        }
    }

    public MainWindowViewModel(ITaskModelFileService taskModelFileService)
    {
        _home = new HomeViewModel(Tasks, Localization);
        _treeView = new TreeViewModel(Localization);
        _profile = new ProfileViewModel(Localization);
        CurrentPage = _home;
        
        TaskModelFileService = taskModelFileService;
        
    }
    
    public async Task LoadFiles()
    {
        var items = await TaskModelFileService.LoadFromFileAsync();
        if (items is not null)
        {
            foreach (var item in items)
            {
                Tasks.AllTasks.Collection.Add(new TaskViewModel(item, Localization));
            }
        }
        
    }

    public async Task SaveFiles()
    {
        await TaskModelFileService.SaveToFileAsync(Tasks.AllTasks.Collection.Select(item => item.TaskModel));
    }
}
