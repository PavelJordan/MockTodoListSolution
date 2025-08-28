
using System.Collections.ObjectModel;
using Avalonia.Media;
using AvaloniaToDoListTrackerAndVisualizer.Providers;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaToDoListTrackerAndVisualizer.ViewModels;

/// <summary>
/// Home view model - where tasks are displayed, and some basic information, like greetings, progress in daily goals, etc.
/// </summary>
public partial class HomeViewModel : ViewModelBase
{
        public TaskListViewModel Tasks { get; }

        public LocalizationProvider Localization { get; }
        
        public ReadOnlyObservableCollection<TaskViewModel> CurrentTaskList { get; private set; }

        public IBrush ReadyButtonBackground
        {
                get { return CurrentTaskList == Tasks.ReadyTasks ? Brushes.DarkGray : Brushes.LightGray; }
        }
        
        public IBrush CompletedButtonBackground
        {
                get { return CurrentTaskList == Tasks.CompletedTasks ? Brushes.DarkGray : Brushes.LightGray; }
        }
        
        public IBrush AllButtonBackground
        {
                get { return CurrentTaskList == Tasks.AllTasksReadOnly ? Brushes.DarkGray : Brushes.LightGray; }
        }

        public HomeViewModel(TaskListViewModel tasks, LocalizationProvider localization)
        {
                Tasks = tasks;
                CurrentTaskList = Tasks.ReadyTasks;
                Localization = localization;
        }

        private void updateCurrentTaskList()
        {
                OnPropertyChanged(nameof(CurrentTaskList));
                OnPropertyChanged(nameof(ReadyButtonBackground));
                OnPropertyChanged(nameof(CompletedButtonBackground));
                OnPropertyChanged(nameof(AllButtonBackground));
        }
        
        /// <summary>
        /// Show ready tasks on CurrentTaskList Property
        /// </summary>
        [RelayCommand]
        private void showReady()
        {
                CurrentTaskList = Tasks.ReadyTasks;
                updateCurrentTaskList();
        }

        /// <summary>
        /// Show completed tasks on CurrentTaskList Property
        /// </summary>
        [RelayCommand]
        private void showCompleted()
        {
                CurrentTaskList = Tasks.CompletedTasks;
                updateCurrentTaskList();
        }

        /// <summary>
        /// Show all tasks on CurrentTaskList Property
        /// </summary>
        [RelayCommand]
        private void showAll()
        {
                CurrentTaskList = Tasks.AllTasksReadOnly;
                updateCurrentTaskList();
        }

        [RelayCommand]
        private void ActionSwitched(bool deletionOn)
        {
                if (deletionOn)
                {
                        foreach (var task in Tasks.AllTasksReadOnly)
                        {
                                task.ActionMode = TaskViewModel.ActionButtonMode.Delete;
                        }
                }
                else
                {
                        foreach (var task in Tasks.AllTasksReadOnly)
                        {
                                task.ActionMode = TaskViewModel.ActionButtonMode.Details;
                        }
                }
        }
}

