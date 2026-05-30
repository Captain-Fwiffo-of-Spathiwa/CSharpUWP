using ProjectManagerProto.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerProto.ViewModels
{
    public class ProjectsViewModel : INotifyPropertyChanged
    {
        private readonly TaskCollection SavedProjects = new();
        public ObservableCollection<DisplayedProjectItem> DisplayedProjects { get; } = new();

        public ICommand ProjectDoubleClickedCommand { get; }
        public ICommand AddProjectCommand { get; }
        public ICommand DeleteCompletedProjectsCommand { get; }

        private ProjectSortMode _sortMode = ProjectSortMode.Name;
        private ProjectFilterMode _filterMode = ProjectFilterMode.All;
        public event PropertyChangedEventHandler? PropertyChanged;


        #if WINDOWS
        private static readonly Dictionary<Project, Microsoft.Maui.Controls.Window> _openWindows = new();
        #endif

        public ProjectsViewModel()
        {
            var sample = SampleData.Create();
            foreach (var list in sample.GetTaskLists())
            {
                if (list is Project project)
                {
                    SavedProjects.AddTaskList(project);
                }
            }
            RefreshProjects();

            ProjectDoubleClickedCommand = new Command<DisplayedProjectItem>(OnProjectDoubleClicked);
            AddProjectCommand = new Command(async () => await AddProjectAsync());
            DeleteCompletedProjectsCommand = new Command(async () => await DeleteCompletedProjects());
        }

        public static void CloseAllProjectWindows()
        {
            #if WINDOWS
            foreach (var win in _openWindows.Values)
            {
                Application.Current.CloseWindow(win);
            }
            _openWindows.Clear();
            #endif
        }

        private async void OnProjectDoubleClicked(DisplayedProjectItem projectItem)
        {
            #if WINDOWS
            Microsoft.UI.Xaml.Window mauiWinUIWindow;

            if (_openWindows.TryGetValue(projectItem.Project, out var existingWindow))
            {
                mauiWinUIWindow = existingWindow.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
                if (mauiWinUIWindow != null)
                {
                    await System.Threading.Tasks.Task.Delay(250);
                    mauiWinUIWindow.Activate();
                }
                return;
            }

            var window = new Microsoft.Maui.Controls.Window
            {
                Page = new ProjectManagerProto.Views.TaskListPage(projectItem.Project),
                Title = "Project Details"
            };

            _openWindows[projectItem.Project] = window;

            window.Destroying += (s, e) =>
            {
                _openWindows.Remove(projectItem.Project);
            };

            Application.Current.OpenWindow(window);
            
            mauiWinUIWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            if (mauiWinUIWindow != null)
            {
                var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(
                    Microsoft.UI.Win32Interop.GetWindowIdFromWindow(
                        WinRT.Interop.WindowNative.GetWindowHandle(mauiWinUIWindow)
                    )
                );
                if (appWindow != null)
                {
                    appWindow.MoveAndResize(new Windows.Graphics.RectInt32(100, 700, 800, 600));
                }
                
                await System.Threading.Tasks.Task.Delay(250);
                mauiWinUIWindow?.Activate();
            }
            #endif
        }

        private async Task AddProjectAsync()
        {
            string result = await Application.Current.MainPage.DisplayPromptAsync(
                "New Project", "Enter project name:", "OK", "Cancel", "Project name");
            if (!string.IsNullOrWhiteSpace(result))
            {
                var project = new Project(result);
                SavedProjects.AddTaskList(project);
                RefreshProjects();
            }
        }

        private async Task DeleteCompletedProjects()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Warning",
                "Are you sure you want to delete all completed projects?",
                "Yes", "No");
            if (confirm)
            {
                SavedProjects.DeleteAllCompletedTaskLists();
                RefreshProjects();
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
            // Get all the saved TaskLists
            var projects = SavedProjects
                .GetTaskLists()
                .Select(taskList => taskList as Project ?? throw new InvalidOperationException("TaskCollection contains a non-Project TaskList."));

            // This switch returns a filtered version of Projects, based on what ProjectFilterMode was set
            projects = _filterMode switch
            {
                ProjectFilterMode.Complete => projects.Where(project => project.PercentComplete >= 100),
                ProjectFilterMode.Incomplete => projects.Where(project => project.PercentComplete < 100),
                _ => projects
            };

            // This switch returns a different ordering of Projects, based on what ProjectSortMode was set
            projects = _sortMode switch
            {
                ProjectSortMode.TaskCount => projects.OrderByDescending(project => project.TotalTasksCount),
                ProjectSortMode.CompletionPercentage => projects.OrderByDescending(project => project.PercentComplete),
                _ => projects.OrderBy(project => project.GetName())
            };

            DisplayedProjects.Clear();

            foreach (var project in projects)
            {
                DisplayedProjects.Add(new DisplayedProjectItem(project));
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public sealed class DisplayedProjectItem
        {
            public DisplayedProjectItem(Project project)
            {
                Project = project;
            }

            public Project Project { get; }

            public string Name => Project.GetName();
            public int TotalTasksCount => Project.TotalTasksCount;
            public int IncompleteTasksCount => Project.IncompleteTasksCount;
            public float PercentComplete => Project.PercentComplete;
        }
    }
}