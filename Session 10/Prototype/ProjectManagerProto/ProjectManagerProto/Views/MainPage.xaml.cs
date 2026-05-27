// Views/MainPage.xaml.cs
using System.Collections.ObjectModel;
using TaskManagement.Models;

namespace ProjectManagerProto.Views;

public partial class MainPage : ContentPage
{
    private readonly TaskCollection _taskCollection = new();
    private readonly ObservableCollection<Project> _projects = new();

    public MainPage()
    {
        InitializeComponent();

        ProjectsList.ItemsSource = _projects;

        AddProject("Prototype Project");
        AddTaskToProject(_projects[0]);
    }

    private void OnAddProjectClicked(object sender, EventArgs e)
    {
        AddProject($"Project {_projects.Count + 1}");
    }

    private void OnAddTaskClicked(object sender, EventArgs e)
    {
        if (ProjectsList.SelectedItem is Project project)
        {
            AddTaskToProject(project);
        }
    }

    private void AddProject(string name)
    {
        var project = new Project(name);

        _taskCollection.AddTaskList(project);
        _projects.Add(project);
    }

    private void AddTaskToProject(Project project)
    {
        project.AddTask(new TaskManagement.Models.Task($"Task {project.TotalTasksCount + 1}"));

        var index = _projects.IndexOf(project);
        _projects.RemoveAt(index);
        _projects.Insert(index, project);
        ProjectsList.SelectedItem = project;
    }
}