using ProjectManagerProto.Models;
using ProjectManagerProto.ViewModels;

namespace ProjectManagerProto.Views
{
    public partial class TaskDialogPage : ContentPage
    {
        public TaskDialogPage(TaskDialogViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}