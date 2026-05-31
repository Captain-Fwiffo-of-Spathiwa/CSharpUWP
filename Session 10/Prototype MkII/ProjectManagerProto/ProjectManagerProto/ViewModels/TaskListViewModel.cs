using Microsoft.UI.Xaml.Controls;
using ProjectManagerProto.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CheckBox = Microsoft.UI.Xaml.Controls.CheckBox;
using DatePicker = Microsoft.UI.Xaml.Controls.DatePicker;
using ScrollBarVisibility = Microsoft.UI.Xaml.Controls.ScrollBarVisibility;
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

        private readonly Microsoft.Maui.Controls.Page Page;


        public TaskListViewModel() {}

        public TaskListViewModel(Project project, Microsoft.Maui.Controls.Page page)
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
#if WINDOWS
            // Use the Page passed to the ctor to find this TaskList's window,
            // so that multiple windows can have their own modal dialogs.
            var nativeWindow = Page.Handler.PlatformView as Microsoft.UI.Xaml.FrameworkElement;
            if (nativeWindow == null) return;

            var description = new TextBox { Header = "Description", Text = taskItem.Description };
            var dueDatePicker = new DatePicker { Header = "Due Date", SelectedDate = taskItem.DueDate };
            var priorityBox = new NumberBox
            {
                Header = "Priority",
                Value = taskItem.PriorityValue,
                Minimum = 1,
                Maximum = 100,
                SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Inline
            };
            var notes = new TextBox { Header = "Notes", Text = taskItem.Notes, AcceptsReturn = true, Height = 60 };
            var completeBox = new CheckBox { Content = "Completed", IsChecked = taskItem.IsComplete };
            var dateCreated = new TextBlock { Text = $"Date Created: {taskItem.DateCreated:yyyy-MM-dd HH:mm}" };

            var dialog = new ContentDialog
            {
                Title = "Edit Task",
                XamlRoot = nativeWindow.XamlRoot,
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                Content = new ScrollViewer
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Content = new StackPanel
                    {
                        Spacing = 16,
                        Children =
                        {
                            description,
                            dueDatePicker,
                            priorityBox,
                            notes,
                            completeBox,
                            dateCreated
                        }
                    }
                }
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                taskItem.Task.Description = description.Text;
                taskItem.Task.DueDate = dueDatePicker.SelectedDate?.DateTime ?? taskItem.Task.DueDate;
                taskItem.Task.TaskPriority = new Priority((int)priorityBox.Value);
                taskItem.Task.Notes = notes.Text;
                taskItem.Task.IsComplete = completeBox.IsChecked ?? false;
                
                RefreshTasks();
                ProjectsViewModel.RefreshProjectsStatic();
            }
#endif
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
                ProjectsViewModel.RefreshProjectsStatic();
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
                ProjectsViewModel.RefreshProjectsStatic();
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
            public DateTime? DueDate => Task.DueDate;
            public int PriorityValue => Task.TaskPriority.Value;
            public string Notes => Task.Notes;
            public bool IsComplete => Task.IsComplete;
            public DateTime DateCreated => Task.DateCreated;
        }
    }
}