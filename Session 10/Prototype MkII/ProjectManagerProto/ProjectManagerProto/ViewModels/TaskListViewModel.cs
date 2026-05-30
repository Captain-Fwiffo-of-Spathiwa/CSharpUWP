using Microsoft.Maui;
using Microsoft.Maui.Controls;
using ProjectManagerProto.Models;
using ProjectManagerProto.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using static ProjectManagerProto.ViewModels.ProjectsViewModel;
using Task = ProjectManagerProto.Models.Task;

namespace ProjectManagerProto.ViewModels
{
    public class TaskListViewModel
    {
        private readonly Project SavedTaskList;
        public ObservableCollection<DisplayedTaskItem> DisplayedTasks { get; } = new();

        public ICommand TaskDoubleClickedCommand { get; }
        public ICommand AddTaskCommand { get; }
        public ICommand DeleteCompletedTasksCommand { get; }

        private TaskSortMode _sortMode = TaskSortMode.Name;
        private TaskFilterMode _filterMode = TaskFilterMode.All;
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Page Page;


        public TaskListViewModel() {}

        public TaskListViewModel(Project project, Page page)
        {
            Page = page;

            SavedTaskList = project;
            RefreshTasks();

            TaskDoubleClickedCommand = new Command<DisplayedTaskItem>(OnTaskDoubleClicked);
            AddTaskCommand = new Command(async () => await AddTaskAsync());
            DeleteCompletedTasksCommand = new Command(async () => await DeleteCompletedTasks());
        }

        private async void OnTaskDoubleClicked(DisplayedTaskItem taskItem)
        {
            var vm = new TaskDialogViewModel(taskItem.Task, Page.Navigation);
            await Page.Navigation.PushModalAsync(new TaskDialogPage(vm));
            if (vm.IsOk)
            {
                // Task was edited, refresh your list
                RefreshTasks();
            }            //Microsoft.UI.Xaml.Window mauiWinUIWindow;

            //if (_openWindows.TryGetValue(projectItem.Project, out var existingWindow))
            //{
            //    mauiWinUIWindow = existingWindow.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            //    if (mauiWinUIWindow != null)
            //    {
            //        await System.Threading.Tasks.Task.Delay(250);
            //        mauiWinUIWindow.Activate();
            //    }
            //    return;
            //}

            //var window = new Microsoft.Maui.Controls.Window
            //{
            //    Page = new ProjectManagerProto.Views.TaskListPage(projectItem.Project),
            //    Title = "Project Details"
            //};

            //_openWindows[projectItem.Project] = window;

            //window.Destroying += (s, e) =>
            //{
            //    _openWindows.Remove(projectItem.Project);
            //};

            //Application.Current.OpenWindow(window);

            //mauiWinUIWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            //if (mauiWinUIWindow != null)
            //{
            //    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(
            //        Microsoft.UI.Win32Interop.GetWindowIdFromWindow(
            //            WinRT.Interop.WindowNative.GetWindowHandle(mauiWinUIWindow)
            //        )
            //    );
            //    if (appWindow != null)
            //    {
            //        appWindow.MoveAndResize(new Windows.Graphics.RectInt32(100, 700, 800, 600));
            //    }

            //    await System.Threading.Tasks.Task.Delay(250);
            //    mauiWinUIWindow?.Activate();
            //}
            //#endif
        }

        private async System.Threading.Tasks.Task AddTaskAsync()
        {
            string result = await Page.DisplayPromptAsync(
                "Add Task",
                "Enter task description:",
                "OK",
                "Cancel",
                "Print flyers by half past 3 tomorrow"
            );

            if (!string.IsNullOrWhiteSpace(result))
            {
                SavedTaskList.AddTask(new("Make biscuits for doggers"));
                RefreshTasks();
            }            
        }

        private async System.Threading.Tasks.Task DeleteCompletedTasks()
        {
            bool confirm = await Page.DisplayAlert(
                "Warning",
                "Are you sure you want to delete all completed tasks?",
                "Yes", "No");

            if (confirm)
            {
                SavedTaskList.DeleteAllCompletedTasks();
                RefreshTasks();
            }
        }

        public bool SortByName
        {
            get => _sortMode == TaskSortMode.Name;
            set { if (value) SetSortMode(TaskSortMode.Name); }
        }

        public bool SortByDueDate
        {
            get => _sortMode == TaskSortMode.DueDate;
            set { if (value) SetSortMode(TaskSortMode.DueDate); }
        }

        public bool SortByPriority
        {
            get => _sortMode == TaskSortMode.Priority;
            set { if (value) SetSortMode(TaskSortMode.Priority); }
        }

        public bool FilterAll
        {
            get => _filterMode == TaskFilterMode.All;
            set { if (value) SetFilterMode(TaskFilterMode.All); }
        }

        public bool FilterComplete
        {
            get => _filterMode == TaskFilterMode.Complete;
            set { if (value) SetFilterMode(TaskFilterMode.Complete); }
        }

        public bool FilterIncomplete
        {
            get => _filterMode == TaskFilterMode.Incomplete;
            set { if (value) SetFilterMode(TaskFilterMode.Incomplete); }
        }

        public bool FilterOverdue
        {
            get => _filterMode == TaskFilterMode.Overdue;
            set { if (value) SetFilterMode(TaskFilterMode.Overdue); }
        }

        private void SetSortMode(TaskSortMode sortMode)
        {
            _sortMode = sortMode;

            OnPropertyChanged(nameof(SortByName));
            OnPropertyChanged(nameof(SortByDueDate));
            OnPropertyChanged(nameof(SortByPriority));

            RefreshTasks();
        }

        private void SetFilterMode(TaskFilterMode filterMode)
        {
            _filterMode = filterMode;

            OnPropertyChanged(nameof(FilterAll));
            OnPropertyChanged(nameof(FilterComplete));
            OnPropertyChanged(nameof(FilterIncomplete));
            OnPropertyChanged(nameof(FilterOverdue));

            RefreshTasks();
        }

        public void RefreshTasks()
        {
            // Get all the saved Tasks
            IEnumerable<Task> tasks = SavedTaskList.GetTasks();

            // This switch returns a filtered set of Tasks, based on what TaskFilterMode was set
            tasks = _filterMode switch
            {
                TaskFilterMode.Complete => tasks.Where(task => task.IsComplete),
                TaskFilterMode.Incomplete => tasks.Where(task => !task.IsComplete),
                TaskFilterMode.Overdue => tasks.Where(task => task.Overdue),
                _ => tasks
            };

            // This switch returns a different ordering of Tasks, based on what TaskSortMode was set
            tasks = _sortMode switch
            {
                TaskSortMode.DueDate => tasks.OrderBy(task => task.DueDate),
                TaskSortMode.Priority => tasks.OrderBy(task => task.TaskPriority.Value),
                _ => tasks.OrderBy(task => task.Description)
            };

            DisplayedTasks.Clear();

            foreach (var task in tasks)
            {
                DisplayedTasks.Add(new DisplayedTaskItem(task));
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public sealed class DisplayedTaskItem
        {
            public DisplayedTaskItem(Task task)
            {
                Task = task;
            }
            
            public Task Task { get; }

            public string Description => Task.Description;
            public DateTime DateCreated => Task.DateCreated;
            public DateTime? DueDate => Task.DueDate;
            public int PriorityValue => Task.TaskPriority.Value;
            public bool IsComplete => Task.IsComplete;
            public string Status => Task.IsComplete ? "Complete" : Task.Overdue ? "Overdue" : "Incomplete";
        }
    }
}