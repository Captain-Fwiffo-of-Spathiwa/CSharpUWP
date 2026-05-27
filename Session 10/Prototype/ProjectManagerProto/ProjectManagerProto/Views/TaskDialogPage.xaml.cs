using ProjectManagerProto.ViewModels;

namespace ProjectManagerProto.Views;

public partial class TaskDialogPage : ContentPage
{
    public TaskDialogPage(TaskViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}