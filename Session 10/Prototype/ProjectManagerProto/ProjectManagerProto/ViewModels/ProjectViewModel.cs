using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TaskManagement.Models;
using ModelTask = TaskManagement.Models.Task;

namespace ProjectManagerProto.ViewModels;

public sealed class ProjectViewModel : INotifyPropertyChanged
{
    private readonly Project _project;
    private TaskSortMode _sortMode = TaskSortMode.Name;
    private TaskFilterMode _filterMode = TaskFilterMode.All;

    public ObservableCollection<TaskItemViewModel> Tasks { get; } = new();

    public string ProjectName => _project.GetName();

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

    public ProjectViewModel(Project project)
    {
        _project = project;
        RefreshTasks();
    }

    public void AddTask(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return;
        }

        var count = _project.TotalTasksCount + 1;

        var task = new ModelTask(description.Trim())
        {
            DueDate = DateTime.Today.AddDays(count),
            TaskPriority = new Priority(count)
        };

        _project.AddTask(task);
        RefreshTasks();
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
        IEnumerable<ModelTask> tasks = _project.GetTasks();

        tasks = _filterMode switch
        {
            TaskFilterMode.Complete => tasks.Where(task => task.IsComplete),
            TaskFilterMode.Incomplete => tasks.Where(task => !task.IsComplete),
            TaskFilterMode.Overdue => tasks.Where(task => task.Overdue.Value),
            _ => tasks
        };

        tasks = _sortMode switch
        {
            TaskSortMode.DueDate => tasks.OrderBy(task => task.DueDate),
            TaskSortMode.Priority => tasks.OrderBy(task => task.TaskPriority.Value),
            _ => tasks.OrderBy(task => task.GetDescription())
        };

        Tasks.Clear();

        foreach (var task in tasks)
        {
            Tasks.Add(new TaskItemViewModel(task));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public sealed class TaskItemViewModel
    {
        public ModelTask Task { get; }

        public string Description => Task.GetDescription();
        public DateTime DateCreated => Task.DateCreated;
        public DateTime DueDate => Task.DueDate.Value;
        public int PriorityValue => Task.TaskPriority.Value;
        public string Status => Task.IsComplete ? "Complete" : Task.Overdue.Value ? "Overdue" : "Incomplete";

        public TaskItemViewModel(ModelTask task)
        {
            Task = task;
        }
    }
}