using ProjectManagerProto.ViewModels;
using TaskManagement.Models;

namespace ProjectManagerProto.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel = new();
    private readonly Dictionary<Project, Window> _openProjectWindows = new();

    public MainPage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
    }

    private async void OnAddProjectClicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("Add Project", "Project name");

        if (!string.IsNullOrWhiteSpace(name))
        {
            _viewModel.AddProject(name);
        }
    }

    private void OnProjectDoubleTapped(object sender, TappedEventArgs e)
    {
        if (sender is not BindableObject bindableObject)
        {
            return;
        }

        if (bindableObject.BindingContext is not MainViewModel.ProjectItemViewModel projectItem)
        {
            return;
        }

        if (_openProjectWindows.ContainsKey(projectItem.Project))
        {
            return;
        }

        var page = new ProjectWindowPage(new ProjectViewModel(projectItem.Project));
        page.ProjectChanged += (_, _) => _viewModel.RefreshProjects();
        var window = new Window(page)
        {
            Title = projectItem.Name
        };

        window.Destroying += (_, _) =>
        {
            _openProjectWindows.Remove(projectItem.Project);
            _viewModel.RefreshProjects();
        };

        _openProjectWindows[projectItem.Project] = window;

        Application.Current?.OpenWindow(window);
    }
}