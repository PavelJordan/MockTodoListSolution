using System.Globalization;
using AvaloniaToDoListTrackerAndVisualizer.Lang;
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

    public MainWindowViewModel()
    {
        _home = new HomeViewModel(Tasks, Localization);
        _treeView = new TreeViewModel(Localization);
        _profile = new ProfileViewModel(Localization);
        CurrentPage = _home;
    }
}
