using ProjectManagerProto.Models;
using ProjectManagerProto.ViewModels;

namespace ProjectManagerProto.Views
{
    public partial class TaskListPage : ContentPage
    {
        public TaskListPage(Project project)
        {
            InitializeComponent();
            BindingContext = new TaskListViewModel(project, this);
        }

        void OnAddButtonPressed(object sender, EventArgs e)
        {
            ((Button)sender).BackgroundColor = Colors.DarkSlateBlue;
        }

        void OnAddButtonReleased(object sender, EventArgs e)
        {
            ((Button)sender).BackgroundColor = Colors.SteelBlue;
        }

        void OnDeleteButtonPressed(object sender, EventArgs e)
        {
            ((Button)sender).BackgroundColor = Colors.DarkRed;
        }

        void OnDeleteButtonReleased(object sender, EventArgs e)
        {
            ((Button)sender).BackgroundColor = Colors.Firebrick;
        }
    }
}