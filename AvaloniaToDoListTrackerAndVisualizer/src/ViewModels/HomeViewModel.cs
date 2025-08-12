
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
        public TaskListViewModel Tasks { get; }
        
        public ReadOnlyObservableCollection<TaskViewModel> CurrentTaskList { get; private set; }

        public HomeViewModel(TaskListViewModel tasks)
        {
                Tasks = tasks;
                CurrentTaskList = Tasks.ReadyTasks;
                OnPropertyChanged(nameof(CurrentTaskList));
        }
                
        [RelayCommand]
        private void showReady()
        {
                CurrentTaskList = Tasks.ReadyTasks;
                OnPropertyChanged(nameof(CurrentTaskList));
        }

        [RelayCommand]
        private void showCompleted()
        {
                CurrentTaskList = Tasks.CompletedTasks;
                OnPropertyChanged(nameof(CurrentTaskList));
        }

        [RelayCommand]
        private void showAll()
        {
                CurrentTaskList = new ReadOnlyObservableCollection<TaskViewModel>(Tasks.AllTasks.Collection);
                OnPropertyChanged(nameof(CurrentTaskList));
        }
}

