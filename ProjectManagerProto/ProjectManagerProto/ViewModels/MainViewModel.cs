using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TaskManagement.Models;

namespace ProjectManagerProto.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly TaskCollection _taskCollection = new();
    private ProjectSortMode _sortMode = ProjectSortMode.Name;
    private ProjectFilterMode _filterMode = ProjectFilterMode.All;
    private ProjectItemViewModel? _selectedProject;

    public ObservableCollection<ProjectItemViewModel> Projects { get; } = new();

    public ProjectItemViewModel? SelectedProject
    {
        get => _selectedProject;
        set
        {
            _selectedProject = value;
            OnPropertyChanged();
        }
    }

    public bool SortByName
    {
        get => _sortMode == ProjectSortMode.Name;
        set { if (value) SetSortMode(ProjectSortMode.Name); }
    }

    public bool SortByTaskCount
    {
        get => _sortMode == ProjectSortMode.TaskCount;
        set { if (value) SetSortMode(ProjectSortMode.TaskCount); }
    }

    public bool SortByCompletionPercentage
    {
        get => _sortMode == ProjectSortMode.CompletionPercentage;
        set { if (value) SetSortMode(ProjectSortMode.CompletionPercentage); }
    }

    public bool FilterAll
    {
        get => _filterMode == ProjectFilterMode.All;
        set { if (value) SetFilterMode(ProjectFilterMode.All); }
    }

    public bool FilterComplete
    {
        get => _filterMode == ProjectFilterMode.Complete;
        set { if (value) SetFilterMode(ProjectFilterMode.Complete); }
    }

    public bool FilterIncomplete
    {
        get => _filterMode == ProjectFilterMode.Incomplete;
        set { if (value) SetFilterMode(ProjectFilterMode.Incomplete); }
    }

    public MainViewModel()
    {
        AddProject("Prototype Project");
    }

    public void AddProject(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        _taskCollection.AddTaskList(new Project(name.Trim()));
        RefreshProjects();
    }

    private void SetSortMode(ProjectSortMode sortMode)
    {
        _sortMode = sortMode;

        OnPropertyChanged(nameof(SortByName));
        OnPropertyChanged(nameof(SortByTaskCount));
        OnPropertyChanged(nameof(SortByCompletionPercentage));

        RefreshProjects();
    }

    private void SetFilterMode(ProjectFilterMode filterMode)
    {
        _filterMode = filterMode;

        OnPropertyChanged(nameof(FilterAll));
        OnPropertyChanged(nameof(FilterComplete));
        OnPropertyChanged(nameof(FilterIncomplete));

        RefreshProjects();
    }

    public void RefreshProjects()
    {
        var projects = _taskCollection
            .GetTaskLists()
            .Select(taskList => taskList as Project ?? throw new InvalidOperationException("TaskCollection contains a non-Project TaskList."));

        projects = _filterMode switch
        {
            ProjectFilterMode.Complete => projects.Where(project => project.PercentComplete >= 100),
            ProjectFilterMode.Incomplete => projects.Where(project => project.PercentComplete < 100),
            _ => projects
        };

        projects = _sortMode switch
        {
            ProjectSortMode.TaskCount => projects.OrderByDescending(project => project.TotalTasksCount),
            ProjectSortMode.CompletionPercentage => projects.OrderByDescending(project => project.PercentComplete),
            _ => projects.OrderBy(project => project.GetName())
        };

        Projects.Clear();

        foreach (var project in projects)
        {
            Projects.Add(new ProjectItemViewModel(project));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public sealed class ProjectItemViewModel
    {
        public Project Project { get; }

        public string Name => Project.GetName();
        public int TotalTasksCount => Project.TotalTasksCount;
        public int IncompleteTasksCount => Project.IncompleteTasksCount;
        public float PercentComplete => Project.PercentComplete;

        public ProjectItemViewModel(Project project)
        {
            Project = project;
        }
    }
}