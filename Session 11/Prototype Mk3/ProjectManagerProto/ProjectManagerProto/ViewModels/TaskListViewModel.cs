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
using TimePicker = Microsoft.UI.Xaml.Controls.TimePicker;



namespace ProjectManagerProto.ViewModels
{
    public enum TaskSortMode
    {
        Name,
        DueDate,
        Priority
    }

    public enum TaskFilterMode
    {
        All,
        Complete,
        Incomplete,
        Overdue
    }

    public class TaskListViewModel
    {
        // Sorting and filtering is done by maintaining a saved list and a presentation list
        private readonly Project SavedTaskList;
        public ObservableCollection<DisplayedTaskItem> DisplayedTasks { get; } = new();

        private TaskSortMode _sortMode = TaskSortMode.Name;
        private TaskFilterMode _filterMode = TaskFilterMode.All;
        public event PropertyChangedEventHandler? PropertyChanged;

        // Buttons and double-clicks
        public ICommand TaskDoubleClickedCommand { get; }
        public ICommand AddTaskCommand { get; }
        public ICommand DeleteCompletedTasksCommand { get; }
        public ICommand DeleteTaskCommand { get; }

        // Hold a reference to this Page for window management
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
            DeleteTaskCommand = new Command(async item => await DeleteTaskAsync(item));
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
            var dueTimePicker = new TimePicker { Header = "Due Time", SelectedTime = taskItem.DueDate?.TimeOfDay };
            var priorityBox = new NumberBox
            {
                Header = "Priority",
                Value = taskItem.PriorityValue,
                Minimum = 1,
                Maximum = 10,
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
                            dueTimePicker,
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

                DateTime? whenDue = null;
                
                // If the user set a date ...
                if (dueDatePicker.SelectedDate != null)
                {
                    // ... add the time the user set, else use 5pm if they didn't set a time
                    if (dueTimePicker.Time >= TimeSpan.Zero)
                    {
                        whenDue = dueDatePicker.SelectedDate?.DateTime.Date + dueTimePicker.Time;
                    }
                    else
                    {
                        whenDue = dueDatePicker.SelectedDate?.DateTime.Date + TimeSpan.FromHours(17);
                    }
                }
                // Else, the user didn't set a date
                else
                {
                    // ... so use a due date only if they did at least set a time
                    if (dueTimePicker.SelectedTime != null)
                    {
                        whenDue = DateTime.Today + dueTimePicker.Time;
                    }
                }
                taskItem.Task.DueDate = whenDue;
                
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
                Task newTask = Helpers.StringParse.ParseNaturalTaskCreation(result);
                SavedTaskList.AddTask(newTask);
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

        private async System.Threading.Tasks.Task DeleteTaskAsync(object item)
        {
            DisplayedTaskItem taskItem = item as DisplayedTaskItem;

            bool confirm = await Page.DisplayAlert(
                "Delete Task",
                "Are you sure you want to delete the selected task?",
                "Yes", "No");
            if (confirm)
            {
                SavedTaskList.RemoveTask(taskItem.Task);
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
                TaskFilterMode.Overdue => tasks.Where(task => task.Overdue && !task.IsComplete),
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