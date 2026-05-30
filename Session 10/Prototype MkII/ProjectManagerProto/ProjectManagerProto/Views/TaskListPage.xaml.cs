namespace ProjectManagerProto.Views
{
    public partial class TaskListPage : ContentPage
    {
        public TaskListPage(object project)
        {
            InitializeComponent();
            BindingContext = project;
        }
    }
}