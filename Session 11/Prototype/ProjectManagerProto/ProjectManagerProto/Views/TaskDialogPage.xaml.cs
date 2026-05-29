using ProjectManagerProto.ViewModels;
using CommunityToolkit.Maui.Views;


namespace ProjectManagerProto.Views;

public partial class TaskDialogPage : Popup
{
    //public event EventHandler? Closed;

    public TaskDialogPage(TaskViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        this.Close();
        //Closed?.Invoke(this, EventArgs.Empty);
        //await Navigation.PopModalAsync();
    }
}