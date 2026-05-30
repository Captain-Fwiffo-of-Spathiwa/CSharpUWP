using Microsoft.Maui.Controls;

namespace ProjectManagerProto.Views
{
    public partial class TaskListPage : ContentPage
    {
        public TaskListPage(object project)
        {
            InitializeComponent();
            // Use the project object as needed, e.g., set BindingContext
            BindingContext = project;
        }
    }
}