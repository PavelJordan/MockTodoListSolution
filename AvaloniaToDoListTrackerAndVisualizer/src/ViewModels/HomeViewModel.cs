
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

/// <summary>
/// Home view model - where tasks are displayed, and some basic information, like greetings, progress in daily goals, etc.
/// </summary>
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
        
        /// <summary>
        /// Show ready tasks on CurrentTaskList Property
        /// </summary>
        [RelayCommand]
        private void showReady()
        {
                CurrentTaskList = Tasks.ReadyTasks;
                OnPropertyChanged(nameof(CurrentTaskList));
        }

        /// <summary>
        /// Show completed tasks on CurrentTaskList Property
        /// </summary>
        [RelayCommand]
        private void showCompleted()
        {
                CurrentTaskList = Tasks.CompletedTasks;
                OnPropertyChanged(nameof(CurrentTaskList));
        }

        /// <summary>
        /// Show all tasks on CurrentTaskList Property
        /// </summary>
        [RelayCommand]
        private void showAll()
        {
                CurrentTaskList = Tasks.AllTasksReadOnly;
                OnPropertyChanged(nameof(CurrentTaskList));
        }
}

