using ProjectManagerProto.ViewModels;

namespace ProjectManagerProto.Views;

public partial class TaskDialogPage : ContentPage
{
    public event EventHandler? Closed;

    public TaskDialogPage(TaskViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        Closed?.Invoke(this, EventArgs.Empty);
        await Navigation.PopModalAsync();
    }
}