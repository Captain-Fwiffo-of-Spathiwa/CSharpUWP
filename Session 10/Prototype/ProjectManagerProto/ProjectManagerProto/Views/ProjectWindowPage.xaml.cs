using ProjectManagerProto.ViewModels;

namespace ProjectManagerProto.Views;

public partial class ProjectWindowPage : ContentPage
{
    private readonly ProjectViewModel _viewModel;
    private bool _taskDialogOpen;
    public event EventHandler? ProjectChanged;

    public ProjectWindowPage(ProjectViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnAddTaskClicked(object sender, EventArgs e)
    {
        var description = await DisplayPromptAsync("Add Task", "Task name");

        if (!string.IsNullOrWhiteSpace(description))
        {
            _viewModel.AddTask(description);
            ProjectChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private async void OnTaskDoubleTapped(object sender, TappedEventArgs e)
    {
        if (_taskDialogOpen)
        {
            return;
        }

        if (sender is not BindableObject bindableObject)
        {
            return;
        }

        if (bindableObject.BindingContext is not ProjectViewModel.TaskItemViewModel taskItem)
        {
            return;
        }

        _taskDialogOpen = true;

        await Navigation.PushModalAsync(new TaskDialogPage(new TaskViewModel(taskItem.Task)));

        _taskDialogOpen = false;
        _viewModel.RefreshTasks();
        ProjectChanged?.Invoke(this, EventArgs.Empty);
    }
}