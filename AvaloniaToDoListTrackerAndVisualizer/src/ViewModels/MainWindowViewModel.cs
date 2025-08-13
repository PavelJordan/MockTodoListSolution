using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

/// <summary>
/// The main window view model. The CurrentPage property is what is displayed.
/// </summary>
public partial class MainWindowViewModel: ViewModelBase
{
    [ObservableProperty]
    private TaskListViewModel _tasks =  new();

    private readonly ViewModelBase _home;
    private readonly ViewModelBase _settings = new SettingsViewModel();
    private readonly ViewModelBase _profile;
    private readonly ViewModelBase _treeView;
    
    [ObservableProperty] private ViewModelBase _currentPage;
    
    [RelayCommand]
    private void GoHome () => CurrentPage = _home;
    
    [RelayCommand]
    private void GoSettings () => CurrentPage = _settings;
    
    [RelayCommand]
    private void GoProfile () => CurrentPage = _profile;
    
    [RelayCommand]
    private void GoTreeView () => CurrentPage = _treeView;

    public MainWindowViewModel()
    {
        _home = new HomeViewModel(Tasks);
        _treeView = new TreeViewModel();
        _profile = new ProfileViewModel();
        CurrentPage = _home;
    }
}
