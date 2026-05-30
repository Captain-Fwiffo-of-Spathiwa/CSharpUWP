using ProjectManagerProto.Models;
using ProjectManagerProto.ViewModels;

namespace ProjectManagerProto.Views
{
    public partial class TaskListPage : ContentPage
    {
        public TaskListPage(Project project)
        {
            InitializeComponent();
            BindingContext = new TaskListViewModel(project);
        }
    }
}